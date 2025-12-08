// Made by Martin M
using UnityEngine;

public static class InteractableExtensions
{
	public static void InitializeIDFromDatabase(this Interactable interactable)
	{
		if (Inventory.TryGetItem(interactable, out InventoryItem item))
		{
			Debug.Log($"Set ID {item.id}");
			if (interactable.GetInteractableData() == null)
			{
				Debug.LogWarning($"[Inventory System (Extensions)] Interactable ({interactable.name})[{item.id}] does not have an interactable data scriptable object");
				return;
			}
			
			interactable.SetID(item.id);

			if (TryGetJournalItem(interactable, out JournalItem journalItem))
			{
				Debug.Log($"Journal found {journalItem.Title}");
			}
			
			return;
		}
		
		Debug.LogWarning($"[Inventory System (Extensions)] Could not find item for {interactable.name}\n" +
		                 "Please make sure the item is in the inventory database (and is a prefab)");
	}
	
	public static bool TryGetJournalItem(this Interactable interactable, out JournalItem journalItem)
	{
		if (JournalsDataManager.TryGetItem(interactable, out journalItem))
		{
			return true;
		}
		
		Debug.LogWarning($"[Inventory System (Extensions)] Could not find journal item for {interactable.name}\n" +
		                 "Please make sure the item is in the inventory (must be a prefab) and its associated ID in the journal database");
		journalItem = null;
		return false;
	}
}