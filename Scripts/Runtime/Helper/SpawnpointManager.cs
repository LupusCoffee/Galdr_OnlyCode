using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public static class SpawnpointManager
{
    [SerializeField]
    static Level[] levels =
    {
        new Level(new List<Spawnpoint>()), // main menu
        new Level(new List<Spawnpoint> // david
        {
            new Spawnpoint(new Vector3(93, 5, 80), Quaternion.identity),
            new Spawnpoint(new Vector3(63, 5, 150), new Quaternion(0, 180, 0, 0)),
            new Spawnpoint(new Vector3(133, 5, 92), new Quaternion(0, -105, 0, 0)),
        }),
        new Level(new List<Spawnpoint> // alex
        {
            new Spawnpoint(new Vector3(-5, 26, 5), Quaternion.identity)
        }),
        new Level(new List<Spawnpoint> // zonghao
        {
            new Spawnpoint(new Vector3(-1.75f, -4.25f, -113), Quaternion.identity)
        })
    };

    public static void ResetSpawnpoints()
    {
        foreach (Level level in levels)
        {
            level.SetSpawnpoint(0);
        }
    }

    public static void SetSpawnpoint(int levelIndex, int spawnpointIndex)
    {
        Debug.Log("Setting spawnpoint for level " + levelIndex + " to spawnpoint " + spawnpointIndex);
        levels[levelIndex].SetSpawnpoint(spawnpointIndex);
    }

    public static void TeleportPlayer(int scene)
    {
        Debug.Log("Player instance found in scene " + scene + " and levels count is " + levels.Length);

        if (scene >= levels.Length) return;
        if (levels[scene] == null) return;

        Level level = levels[scene];

        Debug.Log("Current spawnpoint is " + level.currentSpawnpoint.position);

        Spawnpoint spawnpoint = level.currentSpawnpoint;

        Debug.Log("Setting player spawnpoint to " + spawnpoint.position + " in scene " + scene);

        Player.Instance.transform.position = spawnpoint.position;
        Player.Instance.transform.rotation = spawnpoint.rotation;
    }


    [System.Serializable]
    public class Spawnpoint
    {
        public Vector3 position;
        public Quaternion rotation;

        public Spawnpoint(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }

    [System.Serializable]
    public class Level
    {
        public List<Spawnpoint> availableSpawnpoints;
        public Spawnpoint currentSpawnpoint;

        public Level(List<Spawnpoint> spawnpoints)
        {
            this.availableSpawnpoints = spawnpoints;

            if (this.availableSpawnpoints.Count == 0)
                return;

            currentSpawnpoint = this.availableSpawnpoints[0];
        }

        public void SetSpawnpoint(int index)
        {
            if (index < 0 || index >= availableSpawnpoints.Count)
                return;

            currentSpawnpoint = availableSpawnpoints[index];
        }
    }
}
