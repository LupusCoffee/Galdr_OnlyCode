using UnityEngine;
using static MultiTag;

public class Puzzle_TriggerEvent : PuzzleEvent
{
    [SerializeField] bool isOnlyAffectedByPlayer = true;
    private void OnTriggerEnter(Collider other)
    {
        if (isOnlyAffectedByPlayer) {
            if (other.CompareTag("Player")) {
                TriggerOnButtonPressed();
            }
        }
        else {
            if(other.gameObject == Player.Instance.currentPlayerBody) {
                TriggerOnButtonPressed();
            }
        }
    }
}
