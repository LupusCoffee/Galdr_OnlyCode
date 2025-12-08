// Created by Martin M
using System;
using System.Collections.Generic;
using System.Linq;
using SaveSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
	private const string SaveFileName = "level.dat";
	private const string SceneDataKey = "SceneDatas";
	
	#region Singleton
	public static SaveManager Instance { get; private set; }
	
	public void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		transform.SetParent(null);
		DontDestroyOnLoad(gameObject);
		_booSave = BooSave.Create()
			.WithFileName("level.dat")
			.Build();

		if (!_booSave.TryLoad(SceneDataKey, out Dictionary<int, SavedSceneData> data))
		{
			Debug.LogWarning("Failed to load scene data from file maybe it's the first time playing?");
			return;
		}
		_savedSceneDatas = data;
		#if UNITY_EDITOR
		_initialSceneIndex = SceneManager.GetActiveScene().buildIndex;
		#endif
	}
	
	#endregion
		
	
#if UNITY_EDITOR
		public static bool IsLoadedFromMainMenu => _initialSceneIndex == 0;
#endif


	public static event Action<SavedSceneData> Saving;
	public static event Action<SavedSceneData> Saved;
	public static event Action<SavedSceneData> Loading;
	public static event Action<SavedSceneData> Loaded;
	
	
	private BooSave _booSave;
	private static readonly List<IAutoSaved> SavedTransforms = new();
	
	private Dictionary<int, SavedSceneData> _savedSceneDatas = new();

	
#if UNITY_EDITOR
	private static int _initialSceneIndex;
#endif
	
	public SavedSceneData ActiveSceneData => 
		_savedSceneDatas.GetValueOrDefault(SceneManager.GetActiveScene().buildIndex);

	public static void AddAutoSaved(IAutoSaved autoSaved)
	{
		Debug.Log("Adding auto saved");
		SavedTransforms.Add(autoSaved);
	}
	
	public static void RemoveAutoSaved(IAutoSaved autoSaved)
	{
		SavedTransforms.Remove(autoSaved);
	}
	
	public static void TriggerSave()
	{
		if (Instance == null)
		{
			Debug.LogWarning("Save manager instance not found");
			return;
		}

		Instance.SaveGame();
	}

	public static void TriggerLoad(int index = -1)
	{
		if (Instance == null)
		{
			Debug.LogWarning("Save manager instance not found");
			return;
		}
		
		int sceneIndex = index == -1
			? SceneManager.GetActiveScene().buildIndex
			: index;
		
		Instance.LoadGame(sceneIndex);
	}

	public static void CompleteCurrentLevel()
	{
		if (Instance == null)
		{
			Debug.LogWarning("Save manager instance not found");
			return;
		}
		
		if (Instance.ActiveSceneData == null)
		{
			Debug.LogWarning("Active scene data not found");
			return;
		}

		Instance.ActiveSceneData.Complete();
		Instance.SaveGame();
	}

	/// <summary>
	/// Checks if a level has been unlocked
	/// </summary>
	/// <param name="sceneIndex">level index</param>
	/// <returns>Whether the level has been unlocked or not</returns>
	public static bool HasUnlockedLevel(int sceneIndex)
	{
		if (sceneIndex is 0 or 1) return true;
		if (Instance != null)
		{
			return Instance._savedSceneDatas.TryGetValue(sceneIndex - 1, 
				       out SavedSceneData savedSceneData) 
			       && savedSceneData.IsCompleted;
		}
		Debug.LogWarning("Save manager instance not found");
		return false;
	}
	
	public static void WipeSaveOfScene(int sceneIndex)
	{
		if (Instance == null)
		{
			Debug.LogWarning("Save manager instance not found");
			return;
		}

		if (Instance._savedSceneDatas.ContainsKey(sceneIndex))
		{
			Instance._savedSceneDatas.Remove(sceneIndex);
			Instance._booSave.Save(Instance._savedSceneDatas, SceneDataKey);
		}
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
	
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		LoadGame(scene.buildIndex);
	}
	
	public static void LoadLatestScene()
	{
		if (Instance == null)
		{
			Debug.LogWarning("Save manager instance not found");
			return;
		}
		Instance.LoadLatestSceneInternal();
	}

	private void LoadLatestSceneInternal()
	{
		if (_savedSceneDatas == null)
		{
			Debug.LogWarning("No saved scene data found");
			return;
		}
	
		if (_savedSceneDatas.Count == 0)
		{
			Debug.LogWarning("No saved scene data found loading first scene");
			TransitionAnimator.Instance.LoadGame(1);
			return;
		}
		
		foreach (var savedSceneData in _savedSceneDatas)
		{
			if (savedSceneData.Value.CurrentScene)
			{
				LoadGame(savedSceneData.Value.SceneIndex);
				return;
			}
		}
		Debug.LogWarning("No current scene found");
	}
		
	public void SaveGame(int index = -1)
	{
		var idx = index == -1
			? SceneManager.GetActiveScene().buildIndex
			: index;
		var sceneData = new SavedSceneData(idx);
		Saving?.Invoke(sceneData);
		NotifyWantSave(sceneData);
		_savedSceneDatas[idx] = sceneData;
		
		foreach (KeyValuePair<int, SavedSceneData> savedSceneData in _savedSceneDatas)
		{
			savedSceneData.Value.CurrentScene = savedSceneData.Key == idx;
		}
		
		_booSave.Save(_savedSceneDatas, SceneDataKey);
		Saved?.Invoke(sceneData);
	}
	
	public void LoadGame(int sceneBuildIndex, bool start = false)
	{
		Debug.Log("Loading scene data");
		if (_savedSceneDatas == null)
		{
			Debug.Log("No saved scene data found loading from file");
			return;
		}

		if (sceneBuildIndex == -1)
		{
			Debug.LogWarning("Scene build index is -1 add the scene to the build settings");
			return;
		}
		
		if (!_savedSceneDatas.TryGetValue(sceneBuildIndex, out SavedSceneData startSceneData))
		{
			Debug.LogWarning("No saved scene data found for this scene");
			return;
		}
		
		if (SceneManager.GetActiveScene().buildIndex != sceneBuildIndex)
		{
			TransitionAnimator.Instance.LoadGame(sceneBuildIndex);
			return;
		}

		Loading?.Invoke(startSceneData);
		NotifyWantLoad(startSceneData, start);
		Loaded?.Invoke(startSceneData);
	}

	private void Start()
	{
		LoadGame(SceneManager.GetActiveScene().buildIndex, true);
	}


	private static void NotifyWantSave(SavedSceneData sceneData)
	{
		Debug.Log("Notifying want save");
		foreach (IAutoSaved savedTransform in SavedTransforms)
		{
			savedTransform.UpdateSave(sceneData);
		}
	}
	
	private static void NotifyWantLoad(SavedSceneData sceneData, bool start = false)
	{
		foreach (IAutoSaved savedTransform in SavedTransforms)
		{
			if (!savedTransform.LoadOnStart) continue;
			savedTransform.Load(sceneData);
		}
	}

	public static bool HasSavedData(SavedTransform savedTransform)
	{
		if (Instance != null && Instance.ActiveSceneData != null) return Instance.ActiveSceneData.Contains(savedTransform.Id);
		Debug.LogWarning("Save manager instance or active scene data not found");
		return false;
	}
	
	public static bool TryGetSavedTransformData(SavedTransform savedTransform, out TransformSaveData data)
	{
		if (Instance != null && Instance.ActiveSceneData != null) return Instance.ActiveSceneData.TryLoad(savedTransform.Id, out data);
		Debug.LogWarning("Save manager instance or active scene data not found");
		data = null;
		return false;
	}
}