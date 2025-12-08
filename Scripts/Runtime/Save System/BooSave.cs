// Created by Martin M

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaveSystem.Converters;
using UnityEngine;

namespace SaveSystem
{
	public class BooSaveConfig
	{
		public static readonly BooSaveConfig Default = new();
		
		// public bool UseEncryption { get; set; }   // Sometime in the future?
		
		// public string EncryptionKey { get; set; } // Sometime in the future?
		
		public bool UseCompression { get; set; } = true;
		
		public SaveConfigType Type => (UseCompression ? SaveConfigType.Compression : 0) 
		                              | (UseCompression ? SaveConfigType.Encryption : 0);
	}

	[Flags]
	public enum SaveConfigType
	{
		None = 0,
		Compression = 1 << 0,
		Encryption = 1 << 1,
	}
	
	public class BooSave : IDisposable
	{
		public static BooSave Shared { get; } = new();

		public static bool HasSaveData =>
			File.Exists(Path.Combine(Application.persistentDataPath, "level.dat")) ||
			File.Exists(Path.Combine(Application.persistentDataPath, "inventory.dat"));

		public static Dictionary<int, BooSave> SaveInstances { get; private set; } = new();

		private readonly BaseSaveRoot _saveRoot;

		private BooSave(BaseSaveRoot saveRoot = null)
		{
			_saveRoot = saveRoot ?? BaseSaveRoot.Default;
			
			_saveRoot.Refresh();
			_saveRoot.Load();
			SaveInstances ??= new Dictionary<int, BooSave>();
			SaveInstances[_saveRoot.FileName.GetHashCode()] = this;
		}
		
		public static void WipeAllSaves()
		{
			foreach (BooSave save in SaveInstances.Values)
			{
				save.WipeSave();
			}
			
			WipeSaveByName("inventory.dat");
			WipeSaveByName("level.dat");
			Shared.WipeSave();
		}
		
		public static void WipeSaveByName(string name)
		{
			string filePath = Path.Combine(Application.persistentDataPath, name);
			if (!File.Exists(filePath)) return;
			File.Delete(filePath);
		}
		
		public static BooSaveBuilder Create()
		{
			return new BooSaveBuilder();
		}
		
		private BooSaveConfig Config { get; set; } = BooSaveConfig.Default;
		
		/// <summary>
		/// Whether the save system is initialized or not
		/// </summary>
		public bool IsInitialized => _saveRoot.Data != null;
		
		public bool IsEmpty => IsInitialized && _saveRoot.Data.Count == 0;
		
		/// <summary>
		/// Initializes the save system
		/// </summary>
		public void Initialize(BooSaveConfig config = null)
		{
			Config = config ?? BooSaveConfig.Default;
			_saveRoot.Refresh();
			_saveRoot.Load();
		}
		
		/// <summary>
		/// Save an object to a file
		/// </summary>
		/// <param name="obj">The object to save</param>
		/// <param name="name">Name of the path</param>
		/// <typeparam name="T">Type of the object</typeparam>
		public void Save<T>(T obj, string name)
		{
			_saveRoot.AddOrUpdateKey(obj, name);
			_saveRoot.SaveToDisk();
		}
		
		/// <summary>
		/// Saves the current state of the save system to disk
		/// </summary>
		public void Save()
		{
			_saveRoot.SaveToDisk();
		}
		
		/// <summary>
		/// Updates an object in the save system (in-memory)
		/// </summary>
		/// <param name="obj">Value to save</param>
		/// <param name="name">Path of the value</param>
		/// <typeparam name="T">Type of the value</typeparam>
		public void Update<T>(T obj, string name)
		{
			_saveRoot.AddOrUpdateKey(obj, name);
		}

		/// <summary>
		/// Deletes a key from the save system
		/// </summary>
		/// <param name="name">Name of the path</param>
		public void Delete(string name)
		{
			_saveRoot.DeleteKey(name);
			_saveRoot.SaveToDisk();
		}

		/// <summary>
		/// Wipes the save system (in-memory and disk)
		/// </summary>
		public void WipeSave()
		{
			_saveRoot.Wipe();
			_saveRoot.Refresh();
		}
		
		/// <summary>
		/// Clears the save system (in-memory)
		/// </summary>
		public void Clear()
		{
			_saveRoot.Refresh();
		}
		
		/// <summary>
		/// Load an object from a file
		/// </summary>
		/// <param name="name">Name of the path</param>
		/// <typeparam name="T">Type of the object to load</typeparam>
		/// <returns>The loaded object</returns>
		public T Load<T>(string name)
		{
			object value = _saveRoot.Data[name];
			if (value is T serializable)
			{
				return serializable;
			}

			return value switch
			{
				long l when typeof(T) == typeof(int) => (T)(object)(int)l,
				int i when typeof(T) == typeof(long) => (T)(object)(long)i,
				_                                    => ((JContainer)value).ToObject<T>()
			};
		}
		
		public Type GetType(string name)
		{
			return _saveRoot.Data[name].GetType();
		}
		
		public bool ContainsKey(string name)
		{
			return _saveRoot.Data.ContainsKey(name);
		}

		/// <summary>
		/// Tries to load an object from a file
		/// </summary>
		/// <param name="name">Name of the path</param>
		/// <param name="data">Loaded object data</param>
		/// <typeparam name="T">Type of the object to load</typeparam>
		/// <returns>True whether it was successful or not</returns>
		public bool TryLoad<T>(string name, out T data)
		{
			if (IsEmpty)
			{
				data = default;
				return false;
			}
			if (_saveRoot.Data.TryGetValue(name, out object value) 
			    && (value is T serializable 
			        || value is JContainer container && (serializable = container.ToObject<T>()) != null))
			{
				data = serializable;
				return true;
			}

			data = default;
			return false;
		}

		private class BaseSaveRoot
		{
			public static BaseSaveRoot Default { get; } = new("save.dat");

			public string FileName { get; }

			protected internal BaseSaveRoot(string fileName = "save.dat")
			{
				
				FileName = fileName;
			}


			private string SavePath => Path.Combine(Application.persistentDataPath, FileName);

			internal Dictionary<string, object> Data { get; private set; } = new();

			private static JsonConverter[] Converters { get; } =
			{
				new Vector3Converter(),
				new Vector2Converter(),
				new QuaternionConverter(),
			};

			// ReSharper disable Unity.PerformanceAnalysis
			internal void SaveToDisk()
			{
				if (Data.Count == 0) return;
				string json = JsonConvert.SerializeObject(Data, Converters);
				byte[] bytes = Encoding.UTF8.GetBytes(json);
				using MemoryStream compressed = new();
				using GZipStream gzip = new(compressed, CompressionMode.Compress);
				gzip.Write(bytes, 0, bytes.Length);
				gzip.Close();
				
				byte[] data = compressed.ToArray();
				File.WriteAllBytes(SavePath, data);
				#if DEBUG
					Debug.Log($"Saved [{data.Length}] to: {SavePath}");
				#endif
			}

			/// <summary>
			/// Refreshes the save system (in-memory)
			/// </summary>
			internal void Refresh()
			{
				Data = new Dictionary<string, object>();
			}
			
			internal void Wipe()
			{
				Data.Clear();
				if (!File.Exists(SavePath)) return;
				File.Delete(SavePath);
			}

			/// <summary>
			/// Loads the save data from disk
			/// </summary>
			internal void Load()
			{
				#if DEBUG
					Debug.Log($"Loading from: {SavePath}");
				#endif
					
				if (!File.Exists(SavePath)) return;
				using FileStream fs = File.Open(SavePath, FileMode.Open, FileAccess.Read);
				using GZipStream gzip = new(fs, CompressionMode.Decompress);
				using StreamReader reader = new(gzip);
				string json = reader.ReadToEnd();
				Data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json, Converters);
				#if DEBUG
					Debug.Log(json);
				#endif
			}
			
			/// <summary>
			/// Adds or updates a key in the save system (in-memory)
			/// </summary>
			/// <param name="obj"></param>
			/// <param name="name"></param>
			internal void AddOrUpdateKey(object obj, string name)
			{
				Data[name] = obj;
			}

			/// <summary>
			/// Deletes a key from the save system (in-memory)
			/// </summary>
			/// <param name="name"></param>
			/// <returns></returns>
			internal bool DeleteKey(string name)
			{
				return Data.Remove(name);
			}
		}
		
		public class BooSaveBuilder
		{
			private BooSaveConfig _config = BooSaveConfig.Default;
			private string _fileName;
			
			public BooSaveBuilder WithConfig(BooSaveConfig config)
			{
				_config = config;
				return this;
			}
			
			/// <summary>
			/// Sets the file name of the save file (default is save.dat)
			/// </summary>
			/// <param name="fileName">File name of the save file (default is save.dat)</param>
			/// <returns></returns>
			public BooSaveBuilder WithFileName(string fileName)
			{
				_fileName = fileName;
				return this;
			}
			
			public BooSave Build()
			{
				return new BooSave(new BaseSaveRoot(_fileName ?? "save.dat"));
			}
		}
		
		/// <inheritdoc />
		public void Dispose()
		{
			SaveInstances.Remove(_saveRoot.FileName.GetHashCode());
			_saveRoot.SaveToDisk();
		}
	}
}