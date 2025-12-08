// Made by Martin M

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "JournalDatabase", menuName = "Scriptable Objects/Database/Journals Database", order = 0)]
public class JournalsDatabase : ScriptableObject
{
	public JournalItemData NpcJournal;
	public JournalItemData StoryJournal;
	public JournalItemData SongsJournal;
	public JournalItemData ArtifactsJournal;
	
	#if UNITY_EDITOR

	public void AddJournalItemData()
	{
		if (NpcJournal != null && StoryJournal != null && SongsJournal != null && ArtifactsJournal != null) return;

		string assetPath = AssetDatabase.GetAssetPath(this);
		Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
		// Make sure there are only 4 sub assets
		if (subAssets.Length is 4) return;
		
		if (NpcJournal == null)
		{
			NpcJournal = CreateInstance<JournalItemData>();
			NpcJournal.name = "NpcJournal";
			AssetDatabase.AddObjectToAsset(NpcJournal, this);
			EditorUtility.SetDirty(NpcJournal);
		}
		
		if (StoryJournal == null)
		{
			StoryJournal = CreateInstance<JournalItemData>();
			StoryJournal.name = "StoryJournal";
			AssetDatabase.AddObjectToAsset(StoryJournal, this);
			EditorUtility.SetDirty(StoryJournal);
		}
		
		if (SongsJournal == null)
		{
			SongsJournal = CreateInstance<JournalItemData>();
			SongsJournal.name = "SongsJournal";
			AssetDatabase.AddObjectToAsset(SongsJournal, this);
			EditorUtility.SetDirty(SongsJournal);
		}
		
		if (ArtifactsJournal == null)
		{
			ArtifactsJournal = CreateInstance<JournalItemData>();
			ArtifactsJournal.name = "ArtifactsJournal";
			AssetDatabase.AddObjectToAsset(ArtifactsJournal, this);
			EditorUtility.SetDirty(ArtifactsJournal);
		}
		
		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssets();
		
	}
		
	#endif
		
	public Dictionary<int, JournalItem> GetJournalItems(JournalItemType journalItemType)
	{
		switch (journalItemType)
		{
			case JournalItemType.Artifact:
				return ArtifactsJournal.ItemLookup;
			case JournalItemType.Story:
				return StoryJournal.ItemLookup;
			case JournalItemType.Song:
				return SongsJournal.ItemLookup;
			case JournalItemType.Npc:
				return NpcJournal.ItemLookup;
		}
		
		return null;
	}
		
	public bool TryGetJournalItems(JournalItemType journalItemType, out Dictionary<int, JournalItem> journalItem)
	{
		switch (journalItemType)
		{
			case JournalItemType.Artifact:
				journalItem = ArtifactsJournal.ItemLookup;
				return true;
			case JournalItemType.Story:
				journalItem = StoryJournal.ItemLookup;
				return true;
			case JournalItemType.Song:
				journalItem = SongsJournal.ItemLookup;
				return true;
			case JournalItemType.Npc:
				journalItem = NpcJournal.ItemLookup;
				return true;
			default:
				journalItem = null;
				return false;
		}
	}
}