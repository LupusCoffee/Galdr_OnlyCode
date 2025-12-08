using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class LevelUnlockCheck : MonoBehaviour
{
	public UnityEvent LevelUnlock;
	public UnityEvent LevelLock;
	public bool LevelUnlocked;

	public int LevelId
	{
		get => _levelID;
		set
		{
			_levelID = value;
			CheckLevelUnlock();
		}
	}

	[SerializeField] private TriggerSceneLoader _portalSceneLoader;
	[SerializeField] private int _levelID;
	private BoxCollider _portalCollider;

	private void Awake()
	{
		if (_portalSceneLoader != null)
		{
			_portalCollider = _portalSceneLoader.GetComponent<BoxCollider>();
			if (_portalCollider == null)
			{
				Debug.LogWarning("Portal does not have a box collider");
			}
		}
	}

	private void OnEnable()
	{
		SaveManager.Saved += OnSave;
	}
	
	private void OnDisable()
	{
		SaveManager.Saved -= OnSave;
	}
	
	private void Start()
	{
		CheckLevelUnlock();
	}

	private void OnSave(SavedSceneData savedSceneData)
	{
		CheckLevelUnlock();
	}

	private void CheckLevelUnlock()
	{
		LevelUnlocked = SaveManager.HasUnlockedLevel(_levelID);
		if (_portalSceneLoader != null)
		{
			_portalSceneLoader.CanEnter = LevelUnlocked;
			_portalCollider.isTrigger = LevelUnlocked;
		}
		else
		{
			Debug.LogWarning("Portal Scene Loader is not set");
		}
		if (LevelUnlocked)
		{
			Debug.Log($"Level {_levelID} is unlocked");
			LevelUnlock.Invoke();
		}
		else
		{
			Debug.Log($"Level {_levelID} is locked");
			LevelLock.Invoke();
		}
	}
}