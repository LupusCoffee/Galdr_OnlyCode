using UnityEngine;

public class TemporaryFixForSpawnpoints : MonoBehaviour
{
    void Start()
    {
        SpawnpointManager.ResetSpawnpoints();
    }
}
