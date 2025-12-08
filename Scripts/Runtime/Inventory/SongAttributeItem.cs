// Made by Martin M
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class SongAttributeItem : InventoryItem
{
	[SerializeField] public InteractableSpellAttribute InteractableSpellAttribute = null;

	[SerializeField] public bool AutomaticUnlock = false;
	
	public SO_SpellAttribute SpellAttribute => (InteractableSpellAttribute.GetInteractableData() as SO_InteractableSpellAttributeData)?.GetSpellAttribute();
}