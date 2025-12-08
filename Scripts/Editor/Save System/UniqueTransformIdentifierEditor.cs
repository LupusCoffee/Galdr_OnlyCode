using SaveSystem;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UniqueTransformIdentifier))]
	
public class UniqueTransformIdentifierEditor : Editor
{
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		var props = serializedObject.GetIterator();
		props.NextVisible(true);
		while (props.NextVisible(true))
		{
			if (props.propertyPath == nameof(UniqueTransformIdentifier._manualId)
			    && !((UniqueTransformIdentifier) target)._useManualId) continue;
			GUI.enabled = props.propertyPath != nameof(UniqueTransformIdentifier._generatedId);
			if (props.propertyPath == nameof(UniqueTransformIdentifier.Id))
			{
				GUILayout.Space(10);
			}
			EditorGUILayout.PropertyField(props);
			GUI.enabled = true;
		}

		serializedObject.ApplyModifiedProperties();
		GUILayout.Space(10);
		if (GUILayout.Button("Generate New GUID"))
		{
			((UniqueTransformIdentifier) target).GenerateNewGuid();
			EditorUtility.SetDirty(target);
			serializedObject.Update();
			serializedObject.ApplyModifiedProperties();
		}
	}
}