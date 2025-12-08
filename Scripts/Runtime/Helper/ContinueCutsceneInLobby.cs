using System.Collections;
using UnityEngine;

public class ContinueCutsceneInLobby : MonoBehaviour
{
    public void Start()
    {
        StartCoroutine(WaitForCutscene());
    }

    IEnumerator WaitForCutscene()
    {
        yield return new WaitForSeconds(20);
        ProgressionManager.Instance.SetCurrentStage(ProgressionManager.CurrentStage.LobbyCutscene);
        TransitionAnimator.Instance.LoadGame(4);

        Debug.Log("Cutscene finished, continuing to lobby");
    }
}
