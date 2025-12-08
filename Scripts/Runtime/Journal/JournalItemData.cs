// Made by Martin M
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;


public class JournalItemData : ScriptableObject
{
	[SerializeField] public List<JournalItem> Items = new();

	public Dictionary<int, JournalItem> ItemLookup => Items
		.Where(x => x != null)
		.ToDictionary(x => x.AssociatedID, x => x);
}