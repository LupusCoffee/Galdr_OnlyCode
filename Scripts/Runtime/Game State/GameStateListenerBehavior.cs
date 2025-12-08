// Created by Martin M
public abstract class GameStateListenerBehavior : PausableMonoBehaviour
{
	protected abstract GameStateKind State { get; }

	public abstract void StateEnter();
	protected abstract void StateUpdate();
	public abstract void StateExit();
	
	public override void OnEnable()
	{
		GameState.AddListener(State, this);
		base.OnEnable();
	}
	
	public override void OnDisable()
	{
		GameState.RemoveListener(State, this);
		base.OnDisable();
	}
}