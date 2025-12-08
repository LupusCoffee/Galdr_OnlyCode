// Created by Martin M
using UnityEngine;

public abstract class PausableMonoBehaviour : MonoBehaviour
{
	/// <summary>
	/// Returns true if the game is in slowdown mode.
	/// </summary>
	protected bool IsSlowdown { get; private set; }
	
	/// <summary>
	/// Returns true if the game is paused.
	/// </summary>
	protected bool IsPaused { get; private set; }
	
	/// <summary>
	/// Returns the delta time scaled by 0.1 if the game is in slowdown mode.
	/// </summary>
	protected float ScaledDeltaTime => IsSlowdown
		? Time.deltaTime * 0.1f 
		: Time.deltaTime;
	
	/// <summary>
	/// Returns the fixed delta time scaled by 0.1 if the game is in slowdown mode.
	/// </summary>
	protected float ScaledFixedDeltaTime => IsSlowdown 
		? Time.fixedDeltaTime * 0.1f 
		: Time.fixedDeltaTime;
	
	public virtual void OnEnable()
	{
		GameStateChanged(GameState.CurrentState, GameState.CurrentState);
		GameState.StateChanged += GameStateChanged;
	}

	public virtual void OnDisable()
	{
		GameState.StateChanged -= GameStateChanged;
	}

	/// <summary>
	/// Called when the game is paused.
	/// </summary>
	protected virtual void Paused()
	{
		
	}

	/// <summary>
	/// Called when the game is unpaused.
	/// </summary>
	protected virtual void Unpaused()
	{
		
	}

	/// <summary>
	/// Called when the game state changes.
	/// </summary>
	/// <param name="newstate">New game state</param>
	/// <param name="oldstate">Old game state</param>
	protected virtual void GameStateChanged(GameStateKind newstate, GameStateKind oldstate)
	{
		IsPaused = newstate == GameStateKind.Paused;
		IsSlowdown = newstate == GameStateKind.Slowdown;
		if (IsPaused)
		{
			Paused();
		}
		else
		{
			Unpaused();
		}
	}
}