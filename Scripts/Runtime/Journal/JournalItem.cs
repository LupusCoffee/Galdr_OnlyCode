// Made by Martin M
using System;
using Journal.Attributes;
using UnityEngine;


[Serializable]
public class JournalItem
{
	[SerializeField] public string Title = "Title";
	[SerializeField, TextArea(10, 10)] public string Description = "Description";
	// [SerializeField] public JournalItemType JournalType = JournalItemType.Story;
	[SerializeField] public int AssociatedID = -1;
	
	[SerializeField] public Sprite SpriteIcon;
	
	[JournalVisible(JournalItemType.Story)]    
	public StoryJournalItem StoryData = new();
	
	[HideInInspector] public SongJournalItem SongData = new();
	[HideInInspector] public ArtifactJournalItem ArtifactData = new();

	public JournalItemType JournalType => (JournalItemType)(AssociatedID >> 24);

	public void SetItem(InventoryItem inventoryItem)
	{
		if (inventoryItem == null)
		{
			Debug.LogWarning("[Journal Item] Tried to set item to null");
			return;
		}
        switch (inventoryItem.JournalType)
        {
            case JournalItemType.Song when inventoryItem is SongAttributeItem songAttributeItem:
	            if (songAttributeItem.InteractableSpellAttribute == null ||
	                songAttributeItem.InteractableSpellAttribute.GetInteractableData() == null)
	            {
		            Debug.LogWarning($"[Journal Item] SongAttributeItem ID: {inventoryItem.id} has no SO_InteractableSpellAttributeData or InteractableData");
		            return;
	            }
                SongData = new SongJournalItem(songAttributeItem.InteractableSpellAttribute.GetInteractableData() as SO_InteractableSpellAttributeData);
                break;
            case JournalItemType.Artifact:
                ArtifactData = new ArtifactJournalItem(inventoryItem);
                break;
        }
	}
}