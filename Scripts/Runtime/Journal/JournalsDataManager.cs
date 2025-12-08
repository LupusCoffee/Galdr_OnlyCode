// Made by Martin M
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[DefaultExecutionOrder(1)]
public class JournalsDataManager : MonoBehaviour
{
	private static JournalsDataManager _instance;
	
	public static JournalsDataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindFirstObjectByType<JournalsDataManager>();
			}
			
			return _instance;
		}
	}
	
	[SerializeField] private JournalsDatabase _journalItemData;
	
	[SerializeField] private Dictionary<JournalItemType, Dictionary<int, JournalItem>> _cachedJournalItems = new();
	
	private Inventory _inventory;
	
	private void Awake()
	{
		if (_instance != null)
		{
			Destroy(gameObject);
			return;
		}

		_instance = this;
	}

    private void Start()
    {
        _inventory = FindFirstObjectByType<Inventory>();
        LoadCachedJournalItems();
        LoadInformationFromInventoryDatabase();
    }

    private void LoadInformationFromInventoryDatabase()
	{
		if (_inventory == null)
		{
			Debug.LogWarning("[JournalsDataManager] Inventory is null, please make sure the Inventory component is attached to a GameObject in the scene");
			return;
		}
		
		if (_inventory.Database == null)
		{
			Debug.LogWarning("[JournalsDataManager] Inventory database is null, please assign an inventory database asset");
			return;
		}
		
		foreach (var (_, journalItems) in _cachedJournalItems)
		{
			foreach (var journalItem in journalItems.Values)
			{
				if (journalItem.AssociatedID == -1)
				{
					Debug.LogWarning($"[JournalsDataManager] Journal item [{Enum.GetName(typeof(JournalItemType), journalItem.JournalType)}] {journalItem.Title} has no associated ID");
					continue;
				}
				
				if (_inventory.Database.TryGetItem(journalItem.AssociatedID, out InventoryItem inventoryItem))
				{
					journalItem.SetItem(inventoryItem);
					Debug.Log($"[JournalsDataManager] Loaded journal item {journalItem.Title} from inventory database");
				}
				else
				{
					Debug.LogWarning($"[JournalsDataManager] Could not find item with ID {journalItem.AssociatedID} in the inventory database");
				}
			}
		}
	}
	
	private void LoadCachedJournalItems()
	{
		if (_journalItemData == null)
		{
			Debug.LogWarning("[JournalsDataManager] Journal item data is null, please assign a journal item data asset");
			return;
		}
		
		foreach (JournalItemType value in (JournalItemType[])Enum.GetValues(typeof(JournalItemType)))
		{
			_cachedJournalItems.Add(value, _journalItemData.GetJournalItems(value));
			Debug.Log($"[JournalsDataManager] Loaded {_cachedJournalItems[value].Count} journal items of type {Enum.GetName(typeof(JournalItemType), value)}");
		}
	}
	
	public static bool TryGetJournalItems(JournalItemType journalItemType, out Dictionary<int, JournalItem> journalItem)
	{
		if (_instance == null)
		{
			Debug.LogWarning("[JournalsDataManager] Instance is null when trying to get journal items\n" +
				"please make sure the JournalsDataManager is in the scene");
			journalItem = null;
			return false;
		}

		if (_instance._journalItemData == null)
		{
			Debug.LogWarning("[JournalsDataManager] Journal item data is null, please assign a journal item data asset");
			journalItem = null;
			return false;
		}
		
		if (Instance._journalItemData.TryGetJournalItems(journalItemType, out journalItem))
		{
			return true;
		}

		journalItem = null;
		return false;
	}
	
	public static bool TryGetItem(Interactable interactable, out JournalItem journalItem)
	{
		var id = interactable.GetID();
		JournalItemType journalItemType = (JournalItemType)(interactable.GetID() >> 24);
		
		if (Instance._cachedJournalItems.TryGetValue(journalItemType, out Dictionary<int, JournalItem> journalItems))
		{
			return journalItems.TryGetValue(id, out journalItem);
		}

		journalItem = null;
		return false;
	}

	public static bool TryGetItem(int id, out JournalItem journalItem)
	{
		JournalItemType journalItemType = (JournalItemType)(id >> 24);
		
		if (Instance._cachedJournalItems.TryGetValue(journalItemType, out Dictionary<int, JournalItem> journalItems))
		{
			return journalItems.TryGetValue(id, out journalItem);
		}

		journalItem = null;
		return false;
	}

	public static bool TryGetItem<T>(int id, out T journalItem)
	where T : JournalItem
	{
		JournalItemType journalItemType = (JournalItemType)(id >> 24);
		
		if (Instance._cachedJournalItems.TryGetValue(journalItemType, out Dictionary<int, JournalItem> journalItems))
		{
			if (journalItems.TryGetValue(id, out JournalItem item))
			{
				journalItem = item as T;
				return journalItem != null;
			}
		}

		journalItem = null;
		return false;
	}
}