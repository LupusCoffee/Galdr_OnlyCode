// Made by Martin M
using System;

public static class InteractibleExtensions
{
	public static InventoryInteractibleType GetInteractibleType(this Interactable interactable)
	{
		return interactable switch
		{
			InteractableCollectable _ => InventoryInteractibleType.Artifact,
			InteractableNPC _         => InventoryInteractibleType.Npc,
			_             => throw new NotImplementedException(),
		};
	}
}