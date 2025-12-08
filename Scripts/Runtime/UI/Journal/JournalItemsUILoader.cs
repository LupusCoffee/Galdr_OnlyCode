// Made by Martin M
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class JournalItemsUILoader : MonoBehaviour
{
	[SerializeField] private RectTransform _containerPage;
	[SerializeField] private GameObject _journalItemPrefab;
	[SerializeField] private JournalItemType _journalItemType = JournalItemType.Artifact;
	
	private JournalsDataManager _journalsDataManager;

	public void Start()
	{
		// Load all journal items
		LoadJournalItems();
	}
	
	private void LoadJournalItems()
	{
		// Load all journal items
		if (!JournalsDataManager.TryGetJournalItems(_journalItemType, out var journalItems))
		{
			Debug.LogWarning($"[JournalItemsUILoader] No journal items found for type: {_journalItemType}");
			return;
		}
		
		foreach (var journalItem in journalItems)
		{
			// Instantiate journal item prefab
			var journalItemUiObject = Instantiate(_journalItemPrefab, _containerPage);
			
			// Set journal item data
			if (journalItemUiObject.TryGetComponent(out JournalItemUI journalItemUI))
			{
				Debug.Log($"Set journal item {journalItem.Value.Title} With ID {journalItem.Key}");
				journalItemUI.SetJournalItem(journalItem.Value);
			}
		}
	}
}