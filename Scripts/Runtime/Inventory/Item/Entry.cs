using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class Entry
{
	[field: SerializeField]
	public int Id { get; set; }
	[field: SerializeField]
	public bool Unlocked { get; set; }
	
	public JournalItemType JournalType => (JournalItemType)(Id >> 24);

	public Entry()
	{
		
	}
	
	public Entry(int id, bool unlocked = false)
	{
		Id = id;
		Unlocked = unlocked;
	}
}