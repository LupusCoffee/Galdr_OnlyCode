using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private GameObject player;
    [SerializeField] private List<GameObject> enemies; 
    void Start() {

    }

    void Update() {
        //if (Input.GetKeyDown(KeyCode.E) && Player.Instance.playerState == Player.PlayerState.Rooted) {
        //    if (this.enemies.Count == 0) {
        //        Debug.Log("Empty");
        //    } else {
        //        this.enemies.ElementAt(0).GetComponent<Grabber>().Deactivate();
        //    }
        //}
    }

    public bool AddEnemy(GameObject enemy) {
        this.enemies.Add(enemy);
        return true;
    }

    public bool RemoveEnemy(int index) {
        if (index >= 0 && index < this.enemies.Count) {
            this.enemies.RemoveAt(index);
            return true;
        }
        return false;
    }
    public bool RemoveEnemy(GameObject enemy) {
        return this.enemies.Remove(enemy);
    }
}
