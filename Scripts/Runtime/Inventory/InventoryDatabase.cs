// Made by Martin M
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "InventoryDb", menuName = "Scriptable Objects/Database/Inventory Database", order = 5)]
public class InventoryDatabase : ScriptableObject
{
	[FormerlySerializedAs("Items")]
	[SerializeField]
	public List<InventoryItem> Artifacts = new();

	[SerializeField]
	public List<SongAttributeItem> Songs = new();

	[SerializeField]
	public List<NpcInteractItem> Npcs = new();

	[SerializeField]
	public List<WorldStoryItem> StoryItems = new();
	
	public Dictionary<int, int> ItemLookup => Artifacts
		.Concat(Songs)
		.Concat(Npcs)
		.Concat(StoryItems)
		.Where(x =>
		{
			return (x.id >> 24) switch
			{
				(int)InventoryInteractibleType.Song  => ((SongAttributeItem)x).InteractableSpellAttribute != null,
				(int)InventoryInteractibleType.Story => ((WorldStoryItem)x).worldEventObject != null,
				_                                    => x.interactable != null
			};
		})
		.ToDictionary(x =>
		{
			return (x.id >> 24) switch
			{
				(int)InventoryInteractibleType.Song => ((SongAttributeItem)x).InteractableSpellAttribute.name
					.GetHashCode() + 27,
				(int)InventoryInteractibleType.Story => ((WorldStoryItem)x).worldEventObject.name
					.GetHashCode() + 27,
				_ => x.interactable.name.GetHashCode() + 27
			};
		}, x => x.id);

	
	public Dictionary<SO_SpellAttribute, SongAttributeItem> SpellAttributeLookup => Songs
		.Where(x => x.InteractableSpellAttribute != null)
		.ToDictionary(x => x.SpellAttribute, x => x);

    public Dictionary<SO_WorldEvents, WorldStoryItem> StoryItemLookup => StoryItems
        .Where(x => x.worldEventObject != null)
        .ToDictionary(x => x.WorldEvents, x => x);

    public Dictionary<JournalItemType, InventoryItem> JournalItems => Artifacts
		.Where(x => x.interactable != null)
		.ToDictionary(x => x.JournalType, x => x);
	
	public bool TryGetItem(int id, out InventoryItem item)
	{
		int index = id & 0xFFFFFF; // Masks out the last 8 bits (type) to get the index
		InventoryInteractibleType type = (InventoryInteractibleType)(id >> 24); // Shifts the bits to the right to get the type
		
		if (Artifacts.Count > index)
		{
			switch (type)
			{
				case InventoryInteractibleType.Artifact:
					item = Artifacts[index];
					return item.id == id;
				case InventoryInteractibleType.Song:
					item = Songs[index];
					return item.id == id;
				case InventoryInteractibleType.Npc:
					item = Npcs[index];
					return item.id == id;
				case InventoryInteractibleType.Story:
					item = StoryItems[index];
					return item.id == id;
			}
		}

		item = null;
		return false;
	}

	#if UNITY_EDITOR
	private void OnValidate()
	{
		ValidateDatabase();
	}
	
	public void ValidateDatabase()
	{
		ValidateArtifactIDs();
		ValidateSongIDs();
		ValidateNpcIDs();
		ValidateWorldStory();
	}

	private void ValidateArtifactIDs()
	{
		// check if artifacts is dirty
		if (Artifacts.Count == 0) return;
		HashSet<int> ids = new();
		for (var i = 0; i < Artifacts.Count; i++)
		{
			var artifact = Artifacts[i];
			artifact.id = CalculateID(i, InventoryInteractibleType.Artifact);
			if (artifact.interactable == null) continue;
			int hash = artifact.interactable.name.GetHashCode() + 27;
			if (ids.Add(hash))
			{
				if (artifact.interactable == null)
				{
					Debug.LogWarning($"[Inventory Database] SongAttributeItem ID: {artifact.id} has no Interactable");
					continue;
				}
				artifact.interactable.SetID(artifact.id);
				continue;
			}
			Debug.LogWarning($"[Inventory Database] Duplicate artifact found: {artifact.interactable.name} at index (ID) {i}");
			Artifacts[i].interactable = null;
		}
	}
	
	private void ValidateSongIDs()
	{
		// check if artifacts is dirty
		if (Songs.Count == 0) return;
		if (Songs.Count > 0xFF_FFF)
		{
			Debug.LogError($"[Inventory Database] Too many songs in the database. Maximum is {0xFF_FFF:N0}");;
			return;
		}
		HashSet<int> ids = new();
		for (var i = 0; i < Songs.Count; i++)
		{
			var song = Songs[i];
			song.id = CalculateID(i, InventoryInteractibleType.Song);
			if (song.InteractableSpellAttribute == null) continue;
			int hash = song.InteractableSpellAttribute.name.GetHashCode() + 27;
			if (ids.Add(hash))
			{
				if (song.InteractableSpellAttribute == null)
				{
					Debug.LogWarning($"[Inventory Database] SongAttributeItem ID: {song.id} has no Interactable");
					continue;
				}
				song.InteractableSpellAttribute.SetID(song.id);
				continue;
			}
			Debug.LogWarning($"[Inventory Database] Duplicate song attribute found: {song.InteractableSpellAttribute.name} at index (ID) {i}");
			Songs[i].InteractableSpellAttribute = null;
		}
	}
	
	private void ValidateNpcIDs()
	{
		// check if artifacts is dirty
		if (Npcs.Count == 0) return;
		if (Npcs.Count > 0xFF_FFF)
		{
			Debug.LogError($"[Inventory Database] Too many Npcs in the database. Maximum is {0xFF_FFF:N0}");;
			return;
		}
		HashSet<int> ids = new();
		for (var i = 0; i < Npcs.Count; i++)
		{
			var npc = Npcs[i];
			npc.id = CalculateID(i, InventoryInteractibleType.Song);
			if (npc.interactable == null) continue;
			int hash = npc.interactable.name.GetHashCode() + 27;
			
			if (ids.Add(hash))
			{
				if (npc.interactable == null)
				{
					Debug.LogWarning($"[Inventory Database] SongAttributeItem ID: {npc.id} has no Interactable");
					continue;
				}
				npc.interactable.SetID(npc.id);
				continue;
			}
			Debug.LogWarning($"[Inventory Database] Duplicate npc attribute found: {npc.interactable.name} at index (ID) {i}");
			Npcs[i].interactable = null;
		}
	}
	
	private void ValidateWorldStory()
	{
		// check if artifacts is dirty
		if (StoryItems.Count == 0) return;
		if (StoryItems.Count > 0xFF_FFF)
		{
			Debug.LogError($"[Inventory Database] Too many Story Items in the database. Maximum is {0xFF_FFF:N0}");;
			return;
		}
		HashSet<int> ids = new();
		for (var i = 0; i < StoryItems.Count; i++)
		{
			var story = StoryItems[i];
			story.id = CalculateID(i, InventoryInteractibleType.Story);
			if (story.worldEventObject == null) continue;
			int hash = story.worldEventObject.name.GetHashCode() + 27;
			
			if (ids.Add(hash))
			{
				if (story.worldEventObject == null)
				{
					Debug.LogWarning($"[Inventory Database] SongAttributeItem ID: {story.id} has no Interactable");
					continue;
				}
				
				story.worldEventObject.GetWorldEvent().ID = story.id;
				continue;
			}
			Debug.LogWarning($"[Inventory Database] Duplicate npc attribute found: {story.worldEventObject.name} at index (ID) {i}");
			StoryItems[i].worldEventObject = null;
		}
	}

	public static int CalculateID(int index, InventoryInteractibleType type)
	{
		return index | ((int)type << 24); // 24 bits for the index and 8 bits for the type
	}
	
	#endif
}