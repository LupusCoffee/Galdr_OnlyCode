// Made by Martin M
using System;
using UnityEngine;

[Serializable]
public class SongJournalItem
{
	public SongJournalItem()
	{
		
	}
	
	public SongJournalItem(SO_InteractableSpellAttributeData item)
	{
		Spell = item.GetSpellAttribute();
	}
	
	[HideInInspector] public SO_SpellAttribute Spell;
}