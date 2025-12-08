using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SaveSystem.Editor
{
	public class BooSaveDevEditor : EditorWindow
	{
		[SerializeField] private VisualTreeAsset _mainVisualTree;

		private static BooSaveDevConfig _config;
		
		[MenuItem("Window/Boo Save Dev/Save Remover")]
		private static void ShowWindow()
		{
			var window = GetWindow<BooSaveDevEditor>();
			window.titleContent = new GUIContent("Boo Save Dev");
			window.Show();
		}

		private void OnEnable()
		{
			_config = Resources.Load<BooSaveDevConfig>("BooSaveDevConfig");
		}

		private void CreateGUI()
		{
			VisualElement root = rootVisualElement;
			TemplateContainer visualTree = _mainVisualTree.CloneTree();
			var wipeSavesBtn = visualTree.Q<Button>("btn-wipe-saves");
			wipeSavesBtn.clicked += () =>
			{
				BooSave.WipeAllSaves();
				ShowNotification(new GUIContent("All saves wiped!"));
			};
			visualTree.Bind(new SerializedObject(_config));
			root.Add(visualTree);
		}
	}
}