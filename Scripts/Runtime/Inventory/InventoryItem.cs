// Made by Martin M

using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
	[SerializeField] public int id = -1;
	[SerializeField] public string name = "Empty";
	[SerializeField] public Sprite icon = null;
	[SerializeField] public Interactable interactable = null;

	public InventoryInteractibleType InteractableType => (InventoryInteractibleType)(id >> 24);
	
	public JournalItemType JournalType => (JournalItemType)(id >> 24);

	/// <summary>
	/// Returns true if the item is collected in the inventory
	/// </summary>
	public bool IsCollected => Inventory.HasCollectedItem(id);

	public bool IsUnlocked => Inventory.HasUnlockedItem(id);

	public bool IsLatest => Inventory.IsLatestCollectedItem(id);
	
	/// <summary>
	/// Returns the JournalItem associated with this InventoryItem
	/// </summary>
	public JournalItem JournalItem => Inventory.GetJournalItem(id);
}