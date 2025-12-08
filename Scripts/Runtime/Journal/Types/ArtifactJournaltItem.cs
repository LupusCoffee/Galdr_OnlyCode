// Made by Martin M
using System;

[Serializable]
public class ArtifactJournalItem
{
	public ArtifactJournalItem()
	{
		
	}
	
	public ArtifactJournalItem(InventoryItem item)
	{
		Artifact = item.name;
	}
	
	public string Artifact = "Artifact";
}