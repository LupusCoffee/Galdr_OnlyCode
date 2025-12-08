using Journal.Attributes;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(JournalVisibleAttribute))]
public class JourneyVisibilityDecoratorDrawer : PropertyDrawer
{
	/// <inheritdoc />
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (attribute is not JournalVisibleAttribute visibleAttribute) return;
		var id = property.serializedObject.FindProperty(property.propertyPath
			.Replace(property.name, "AssociatedID")
		).intValue;
		
		var itemJournalType = (JournalItemType)(id >> 24);
		if (itemJournalType != visibleAttribute.JournalItemType) return;
		
		// Draw the property
		EditorGUI.PropertyField(position, property, label, true);
	}

	/// <inheritdoc />
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (attribute is not JournalVisibleAttribute visibleAttribute) return 0;
		var id = property.serializedObject.FindProperty(property.propertyPath
			.Replace(property.name, "AssociatedID")
		).intValue;
		
		var itemJournalType = (JournalItemType)(id >> 24);
		if (itemJournalType != visibleAttribute.JournalItemType) return 0;
		
		// Calculate the height
		return EditorGUI.GetPropertyHeight(property, label);
	}
}