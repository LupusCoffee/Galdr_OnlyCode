using UnityEngine;

public class TriggerSceneLoader : MonoBehaviour
{
    public bool CanEnter = true;
    
    [SerializeField] int sceneToLoad = 0;
    [SerializeField] int spawnpointIndex = 0;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && CanEnter)
        {
            SpawnpointManager.SetSpawnpoint(sceneToLoad, spawnpointIndex);
            TransitionAnimator.Instance.LoadGame(sceneToLoad);
        }
    }
}
