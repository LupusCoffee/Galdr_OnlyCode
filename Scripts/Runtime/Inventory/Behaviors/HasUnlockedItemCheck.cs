using System;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class HasUnlockedItemCheck : MonoBehaviour
{
	public int Id;

	public bool CheckUnlock = false;
	public bool CheckCollect = true;
	
	private Collider _collider;
	private ParticleSystem[] _particleSystems;

	public bool IsItemActive => _collider.enabled;

    private void Awake()
	{
		_collider = GetComponent<Collider>();
		_particleSystems = GetComponentsInChildren<ParticleSystem>();
		SetItemActive(false);
	}

	private void OnEnable()
	{
		Inventory.ItemCollected += OnItemCollected;
	}

	private void Start()
	{
		switch (CheckUnlock)
		{
			// If both checks are enabled, we want to check if the item is collected AND unlocked
			case true when CheckCollect:
				SetItemActive(!(Inventory.HasUnlockedItem(Id) && Inventory.HasCollectedItem(Id)));
				break;
			case true when !CheckCollect:
				SetItemActive(!Inventory.HasUnlockedItem(Id));
				break;
			case false when CheckCollect:
				SetItemActive(!Inventory.HasCollectedItem(Id));
				break;
		}
	}

	private void OnDestroy()
	{
		Inventory.ItemCollected -= OnItemCollected;
	}

	private void OnItemCollected(InventoryItem inventoryItem)
	{
		Debug.Log($"Checking item {Id} against {inventoryItem.id}");
		if (inventoryItem.id != Id) return;
		switch (CheckUnlock)
		{
			case true when CheckCollect:
				SetItemActive(!(inventoryItem.IsUnlocked && inventoryItem.IsCollected));
				break;
			case true:
				SetItemActive(inventoryItem.IsUnlocked);
				break;
			default:
			{
				SetItemActive(CheckCollect && inventoryItem.IsCollected);
				break;
			}
		}
	}
	
	private void SetItemActive(bool active)
	{
		Debug.Log($"Setting item {Id} active: {active}");
		_collider.enabled = active;
		
		foreach (ParticleSystem _particleSystem in _particleSystems)
		{
			if (active)
			{
				_particleSystem.Play();
			}
			else
			{
				_particleSystem.Stop(true, 
					ParticleSystemStopBehavior.StopEmittingAndClear);
			}
		}
	}
}