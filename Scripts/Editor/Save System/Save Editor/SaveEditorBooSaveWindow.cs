using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SaveSystem.Editor.Save_Editor
{
	public class SaveEditorBooSaveWindow : EditorWindow
	{
		[SerializeField] private VisualTreeAsset _mainVisualTree;
		
		private FileSystemWatcher _fileSystemWatcher;
		
		[MenuItem("Window/Boo Save Dev/Save Editor")]
		private static void ShowWindow()
		{
			var window = GetWindow<SaveEditorBooSaveWindow>();
			window.titleContent = new GUIContent("Boo Save - Save Editor");
			window.Show();
		}

		private void OnEnable()
		{
			// Create a FileSystemWatcher to monitor changes in the save directory
			_fileSystemWatcher = new FileSystemWatcher
			{
				Path = Application.persistentDataPath,
				NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
				Filter = "*.dat"
			};
			_fileSystemWatcher.Changed += OnSaveFileChanged;
			_fileSystemWatcher.Created += OnSaveFileChanged;
			_fileSystemWatcher.Deleted += OnSaveFileChanged;
			_fileSystemWatcher.EnableRaisingEvents = true;
		}

		private void OnDisable()
		{
			// Clean up the FileSystemWatcher
			if (_fileSystemWatcher == null) return;
			_fileSystemWatcher.Changed -= OnSaveFileChanged;
			_fileSystemWatcher.Created -= OnSaveFileChanged;
			_fileSystemWatcher.Deleted -= OnSaveFileChanged;
			_fileSystemWatcher.Dispose();
			_fileSystemWatcher = null;
		}

		private void OnSaveFileChanged(object sender, FileSystemEventArgs e)
		{
			// Handle the save file change event
			Debug.Log($"Save file changed: {e.FullPath}");
			// You can refresh the UI or perform other actions here
		}

		private void CreateGUI()
		{
			VisualElement root = rootVisualElement;
			TemplateContainer visualTree = _mainVisualTree.CloneTree();
			
			root.Add(visualTree);
		}
	}
}