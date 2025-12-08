// Created by Martin M
using System;

public sealed class FiniteStateMachine<T> where T : Enum
{
	public T CurrentState { get; private set; }
	
	public event StateChangedHandler<T> StateChanged;
	
	public FiniteStateMachine(T initialState)
	{
		CurrentState = initialState;
	}
	
	public void ChangeState(T newState)
	{
		OnStateChanged(newState);
		CurrentState = newState;
	}

	private void OnStateChanged(T newState)
	{
		StateChanged?.Invoke(newState, CurrentState);
	}
}