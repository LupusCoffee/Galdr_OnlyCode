using UnityEngine;

public class ChangeGameStateOnLoad : MonoBehaviour
{
    public GameStateKind StateKind;

    private void Start()
    {
        GameState.ChangeState(StateKind);
    }
}
