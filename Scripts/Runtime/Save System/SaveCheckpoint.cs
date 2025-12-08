using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SaveCheckpoint : MonoBehaviour
{
	private BoxCollider _boxCollider;
	
	private void Awake()
	{
		_boxCollider = GetComponent<BoxCollider>();
		_boxCollider.isTrigger = true;
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			SaveManager.TriggerSave();
		}
	}
}