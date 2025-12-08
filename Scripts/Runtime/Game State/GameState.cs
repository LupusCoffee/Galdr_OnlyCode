// Created by Martin M

using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
	private static FiniteStateMachine<GameStateKind> StateMachine { get; } = new(GameStateKind.MainMenu);
	
	public static GameStateKind CurrentState => StateMachine.CurrentState;
	
	public static event StateChangedHandler<GameStateKind> StateChanged
	{
		add => StateMachine.StateChanged += value;
		remove => StateMachine.StateChanged -= value;
	}

	private static readonly Dictionary<GameStateKind, List<GameStateListenerBehavior>> GameStateListeners = new();
	
	/// <summary>
	/// Changes the current state of the game.
	/// </summary>
	/// <param name="newState">State to change to</param>
	public static void ChangeState(GameStateKind newState)
	{
		NotifyListeners(newState, StateMachine.CurrentState);
		StateMachine.ChangeState(newState);
	}

	/// <summary>
	/// Pauses the game.
	/// </summary>
	public static void Pause()
	{
		if (StateMachine.CurrentState != GameStateKind.Playing) return;
		ChangeState(GameStateKind.Paused);
	}
	
	/// <summary>
	/// Unpauses the game.
	/// </summary>
	public static void Unpause()
	{
		if (StateMachine.CurrentState != GameStateKind.Paused) return;
		ChangeState(GameStateKind.Playing);
	}
	
	public static void AddListener(GameStateKind state, GameStateListenerBehavior listenerBehavior)
	{
		if (!GameStateListeners.ContainsKey(state))
		{
			GameStateListeners[state] = new List<GameStateListenerBehavior>();
		}
		GameStateListeners[state].Add(listenerBehavior);
	}
	
	public static void RemoveListener(GameStateKind state, GameStateListenerBehavior listenerBehavior)
	{
		if (GameStateListeners.TryGetValue(state, out List<GameStateListenerBehavior> stateListener))
		{
			stateListener.Remove(listenerBehavior);
		}
	}
	
	public static void NotifyListeners(GameStateKind currentState, GameStateKind oldState)
	{
		var listenersToRemove = new List<GameStateListenerBehavior>();
		if (GameStateListeners.TryGetValue(oldState, out List<GameStateListenerBehavior> stateListeners))
		{
			foreach (GameStateListenerBehavior listener in stateListeners)
			{
				if (listener == null)
				{
					listenersToRemove.Add(listener);
					continue;
				}
				listener.StateExit();
			}
		}
		
		foreach (GameStateListenerBehavior listener in listenersToRemove)
		{
			GameStateListeners[oldState].Remove(listener);
		}
		
		listenersToRemove.Clear();
		
		if (GameStateListeners.TryGetValue(currentState, out stateListeners))
		{
			foreach (GameStateListenerBehavior listener in stateListeners)
			{
				if (listener == null)
				{
					listenersToRemove.Add(listener);
					continue;
				}
				listener.StateEnter();
			}
		}
		
		foreach (GameStateListenerBehavior listener in listenersToRemove)
		{
			GameStateListeners[currentState].Remove(listener);
		}
	}
}
