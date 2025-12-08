// Made by Martin M
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;


[CustomEditor(typeof(InventoryDatabase))]
public class InventoryDatabaseEditor : Editor
{
	[SerializeField]
	private VisualTreeAsset _databaseEditor;
	
	public override VisualElement CreateInspectorGUI()
	{
		VisualElement root = new();
		if (_databaseEditor == null)
		{
			root.Add(new Label("DatabaseEditor visual tree asset is not set"));
			return root;
		}
		TemplateContainer editor = _databaseEditor.CloneTree();
		
		var button = editor.Q<Button>("btn-regenerate");
		button.clicked += () =>
		{
			InventoryDatabase database = target as InventoryDatabase;
			if (database == null) return;
			database.ValidateDatabase();
		};
		
		SerializedObject serializedTarget = new(target);
		
		editor.Bind(serializedTarget);
		root.Add(editor);
		return root;
	}

	private void OnValidate()
	{
		Debug.Log("OnValidate");
	}
}