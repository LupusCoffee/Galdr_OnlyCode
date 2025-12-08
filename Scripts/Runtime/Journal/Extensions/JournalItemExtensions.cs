public static class JournalItemExtensions
{
	public static bool IsCollected(this InventoryItem item)
	{
		return Inventory.HasCollectedItem(item.id);
	}
	public static bool IsUnlocked(this InventoryItem item)
	{
		return Inventory.HasUnlockedItem(item.id);
	}
	
	public static bool IsCollected(this JournalItem journalItem)
	{
		return Inventory.HasCollectedItem(journalItem.AssociatedID);
	}
	
	public static bool IsUnlocked(this JournalItem journalItem)
	{
		return Inventory.HasUnlockedItem(journalItem.AssociatedID);
	}

	public static bool IsLatest(this JournalItem journalItem)
	{
		return Inventory.IsLatestCollectedItem(journalItem.AssociatedID);
	}
}