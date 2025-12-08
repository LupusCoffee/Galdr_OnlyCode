using System;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class WorldStoryItem : InventoryItem
{
	[SerializeField] public WorldEventObject worldEventObject = null;

    public SO_WorldEvents WorldEvents => worldEventObject.GetWorldEvent();
}