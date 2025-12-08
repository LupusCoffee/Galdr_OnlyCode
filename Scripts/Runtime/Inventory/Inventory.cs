// Made by Martin M

using System;
using System.Collections.Generic;
using SaveSystem;
using UnityEngine;
using UnityEngine.Serialization;

[DefaultExecutionOrder(-100)]
public class Inventory : MonoBehaviour, IAutoSaved
{
	private const string InventorySaveKey = "inventoryItems";
	
	private static Inventory _instance;
	private static Inventory Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindFirstObjectByType<Inventory>();
			}

			return _instance;
		}
	}
	private static Dictionary<int, int> _lookupItemTable = new();
	public static Dictionary<SO_SpellAttribute, SongAttributeItem> LookupSpellAttributeTable = new();
    public static Dictionary<SO_WorldEvents, WorldStoryItem> LookupWorldStoryTable = new();


    public static event Action<InventoryItem> ItemCollected;
	
	[FormerlySerializedAs("database")]
	[SerializeField] private InventoryDatabase _database;
	[SerializeField] private List<Entry> _inventory = new();
	private Dictionary<int, Entry> _lookupEntries = new();
	private HashSet<int> _inventorySet = new();

	private int _latestCollectedItemId;
	
	public InventoryDatabase Database => _database;
	
	public Dictionary<JournalItemType, List<int>> Items = new();
	
	private BooSave _booSave;
	
	public static bool IsLatestCollectedItem(int id)
	{
		return Instance._latestCollectedItemId == id;
	}

	
	public bool HasItem(int id)
	{
		return _inventorySet.Contains(id);
	}
	
	public static bool HasCollectedItem(int id)
	{
		return Instance != null && Instance.HasItem(id);
	}
	
	public static bool TryGetItem(Interactable interactable, out InventoryItem item)
	{
		return Instance._database.TryGetItem(interactable.GetID(), out item);
	}

    public static bool TryGetItemFromWorldStory(SO_WorldEvents worldEvent, out WorldStoryItem item) {
        item = null;
        return LookupWorldStoryTable.TryGetValue(worldEvent, out item);
    }

    public static bool TryGetItemFromSpellAttribute(SO_SpellAttribute attribute, out SongAttributeItem item)
	{
		item = null;
		return LookupSpellAttributeTable.TryGetValue(attribute, out item);
	}
	
	public static bool TryGetItem(int id, out InventoryItem item)
	{
		return Instance._database.TryGetItem(id, out item);
	}


	/// <summary>
	/// Tries to collect an item from an Item ID
	/// </summary>
	/// <param name="id">Item ID</param>
	/// <returns></returns>
	public static bool TryCollectItem(int id)
	{
		//Debug.Log("[Inventory] Trying to collect item");
		if (_instance == null)
		{
			//Debug.LogWarning("[Inventory] Inventory instance is null cannot collect item\n" +
			//                 "Make sure the Inventory component is attached to a GameObject in the scene");
			return false;
		}
		Instance.CollectItem(id);
		return false;
	}


	/// <summary>
	/// Tries to collect an item from a Spell Attribute
	/// </summary>
	/// <param name="spellAttribute">Spell attribute to collect</param>
	/// <returns>Returns true whether the item was successfully collected or not</returns>
	public static bool TryCollectItem(SO_SpellAttribute spellAttribute)
	{
		if (_instance == null)
		{
			//Debug.LogWarning("[Inventory] Inventory instance is null cannot collect item\n" +
			//                 "Make sure the Inventory component is attached to a GameObject in the scene");
			return false;
		}
		
		if (LookupSpellAttributeTable.TryGetValue(spellAttribute, out SongAttributeItem item))
		{
			if (!item.IsCollected) return Instance.CollectItem(item.id);
			//Debug.LogWarning("[Inventory] Item is already collected");
			return false;
		}
		
		//Debug.LogWarning("[Inventory] Could not find item in lookup table");
		return false;
	}


	public static bool TryCollectItem(Interactable interactable, bool unlock = false)
	{
		//Debug.Log("[Inventory] Trying to collect item");
		if (_instance == null)
		{
			//Debug.LogWarning("[Inventory] Inventory instance is null cannot collect item\n" +
			//                 "Make sure the Inventory component is attached to a GameObject in the scene");
			return false;
		}
		Instance.CollectItem(interactable, unlock);
		return false;
	}

    public static bool TryCollectItem(SO_WorldEvents worldEvent) {
	    if (_instance == null)
	    {
		    Debug.LogWarning("[Inventory (World Event)] Inventory instance is null cannot collect item\n" +
		                     "Make sure the Inventory component is attached to a GameObject in the scene");
		    return false;
	    }

        if (LookupWorldStoryTable.TryGetValue(worldEvent, out WorldStoryItem item)) {
	        if (!item.IsCollected)
	        {
		        Debug.Log("[Inventory (World Event)] Collecting item");
		        return Instance.CollectItem(item.id);
	        }
	        Debug.LogWarning("[Inventory (World Event)] Item is already collected");
            return false;
        }
        
        Debug.LogWarning("[Inventory (World Event)] Could not find item in lookup table");
        return false;
    }


    public static bool TryUnlockNpc(Interactable interactable)
	{
		if (_instance == null)
		{
			//Debug.LogWarning("[Inventory] Inventory instance is null cannot unlock npc\n" +
			//                 "Make sure the Inventory component is attached to a GameObject in the scene");
			return false;
		}
		_instance._database.TryGetItem(interactable.GetID(), out InventoryItem item);
		Debug.Log("[Inventory] Unlocking NPC - Not implemented");
		return false;
	}

	private void Awake()
	{
		_lookupItemTable = _database.ItemLookup;
		LookupSpellAttributeTable = _database.SpellAttributeLookup;
		LookupWorldStoryTable = _database.StoryItemLookup;
		_booSave = BooSave.Create()
			.WithFileName("inventory.dat")
			.Build();
		
		LoadInventory();
	}

	private void OnEnable()
	{
		SaveManager.AddAutoSaved(this);
	}
	
	private void OnDisable()
	{
		SaveManager.RemoveAutoSaved(this);
	}

	public void SaveInventory()
	{
		_booSave.Save(_inventory, InventorySaveKey);
	}
	
	public bool CollectItem(Interactable interactable, bool unlock = false)
	{
		return TryGetItem(interactable, out InventoryItem item) && _instance.CollectItem(item.id, unlock);
	}
	
	public bool CollectItem(int id, bool unlock = false)
	{
		if (TryGetItem(id, out InventoryItem item))
		{
			//Debug.Log("[Inventory] Found item in database");
			if (!_inventorySet.Add(item.id))
			{
				//Debug.LogWarning($"Item with id {item.id} is already in inventory");
				return false;
			}
			_inventory.Add(new Entry(item.id, unlock));
			if (Items.TryGetValue(item.JournalType, out List<int> items))
			{
				items.Add(item.id);
			}
			else
			{
				Items.Add(item.JournalType, new List<int> {item.id});
			}
			SaveInventory();
			OnItemCollected(item);
			return true;
		}

		Debug.LogWarning($"Failed to collect item with id {id} item does not exist in database");
		return false;
	}
	
	public bool CollectSpellAttribute(InteractableSpellAttribute interactable, bool unlock = false)
	{
		if (interactable.GetInteractableData() is not SO_InteractableSpellAttributeData spellAttributeData) return false;
		var spellAttribute = spellAttributeData.GetSpellAttribute();
		if (spellAttribute == null) return false;
		//Debug.Log("[Inventory] Found spell attribute in database");
		if (TryGetItem(interactable, out InventoryItem item))
		{
			return CollectItem(item.id, unlock);
		}

		//Debug.LogWarning($"Failed to collect item with id {interactable.GetID()} item does not exist in database");
		return false;
	}
	
	public void RemoveItem(int id)
	{
		var index = _inventory.FindIndex(x => x.Id == id);
		if (index == -1)
		{
			//Debug.LogWarning($"Item with id {id} does not exist in inventory");
			return;
		}
		_inventory.RemoveAt(index);
	}

	private bool LoadAutomaticUnlockedItems(ref HashSet<int> hashSet,
		ref List<Entry> inventory, 
		ref Dictionary<JournalItemType, List<int>> itemsDictionary,
		ref Dictionary<int, Entry> lookupEntries)
	{
		bool hasUnlockedItems = false;
		foreach (SongAttributeItem item in _database.Songs)
		{
			if (!item.AutomaticUnlock) continue;
			if (!hashSet.Add(item.id)) continue;
			Entry entry = new Entry(item.id, true);
			inventory.Add(entry);
			lookupEntries.Add(item.id, entry);
			if (itemsDictionary.TryGetValue(item.JournalType, out List<int> items))
			{
				items.Add(item.id);
			}
			else
			{
				itemsDictionary.Add(item.JournalType, new List<int> {item.id});
			}
			hasUnlockedItems = true;
		}
		
		return hasUnlockedItems;
	}
	
	public void LoadInventory()
	{
		var items = new Dictionary<JournalItemType, List<int>>();
		var lookup = new Dictionary<int, Entry>();
		var hashSet = new HashSet<int>();
		
		if (!_booSave.TryLoad(InventorySaveKey, out List<Entry> inventory))
		{
			//Debug.LogError("Failed to load inventory items from save file");
			
			// Load automatic unlocked items
			if (!_booSave.IsEmpty) return;
			inventory = new List<Entry>();
			if (!LoadAutomaticUnlockedItems(ref hashSet, ref inventory, ref items, ref lookup)) return;
			_inventorySet = hashSet;
			_inventory = inventory;
			Items = items;
			_lookupEntries = lookup;
			SaveInventory();
			AnnounceItemsCollected(inventory);
			return;
		}
		
		// validate items
		var invalidItemsIndex = new List<int>();
		for (var index = 0; index < inventory.Count; index++)
		{
			Entry entry = inventory[index];
			if (_database.TryGetItem(entry.Id, out InventoryItem item))
			{
				if (!hashSet.Add(entry.Id))
				{
					//Debug.LogWarning($"Item with id {entry} is already in inventory");
					invalidItemsIndex.Add(index);
					continue;
				}
				
				lookup.Add(entry.Id, entry);

				if (items.TryGetValue(item.JournalType, out List<int> itemsList))
				{
					itemsList.Add(entry.Id);
				}
				else
				{
					items.Add(item.JournalType, new List<int> { entry.Id });
				}

			#if DEBUG
				//Debug.Log(
				//	$"Loaded item with id {entry} and type {Enum.GetName(typeof(JournalItemType), item.JournalType)}");
			#endif

				continue;
			}

			//Debug.LogWarning($"Failed to load item with id {entry}");
			invalidItemsIndex.Add(entry.Id);
		}
		
		// Load automatic unlocked items
		LoadAutomaticUnlockedItems(ref hashSet,
			    ref inventory,
			    ref items,
			    ref lookup);

		if (invalidItemsIndex.Count > 0)
		{
			foreach (int index in invalidItemsIndex)
			{
				inventory.RemoveAt(index);
			}
		}
		
		_inventorySet = hashSet;
		_inventory = inventory;
		Items = items;
		_lookupEntries = lookup;
		SaveInventory();
		AnnounceItemsCollected(inventory);
	}

	private void AnnounceItemsCollected(in List<Entry> items)
	{
		foreach (Entry entry in items)
		{
			if (_database.TryGetItem(entry.Id, out InventoryItem item))
			{
				OnItemCollected(item);
			}
		}
	}

	/// <inheritdoc />
	public bool LoadOnStart { get; set; }

	/// <inheritdoc />
	public void Save(bool force = false)
	{
		SaveInventory();
	}

	/// <inheritdoc />
	public void UpdateSave(SavedSceneData sceneData, bool force = false)
	{
		SaveInventory();
	}

	/// <inheritdoc />
	public void Load(SavedSceneData sceneData, bool force = false)
	{
		LoadInventory();
	}

	private static void OnItemCollected(InventoryItem obj)
	{
		ItemCollected?.Invoke(obj);
	}

	public static JournalItem GetJournalItem(int id)
	{
		JournalsDataManager.TryGetItem(id, out JournalItem journalItem);
		return journalItem;
	}

	public static bool HasUnlockedItem(int id)
	{
		return Instance._lookupEntries.TryGetValue(id, out Entry entry) && entry.Unlocked;
	}
	
	public static bool TryUnlockItem(int id)
	{
		if (_instance == null)
		{
			//Debug.LogWarning("[Inventory] Inventory instance is null cannot unlock item\n" +
			//                 "Make sure the Inventory component is attached to a GameObject in the scene");
			return false;
		}
		if (_instance._lookupEntries.TryGetValue(id, out Entry entry))
		{
			entry.Unlocked = true;
			_instance.SaveInventory();
			return true;
		}
		Debug.LogWarning($"Failed to unlock item with id {id} item does not exist in inventory");
		return false;
	}
}
