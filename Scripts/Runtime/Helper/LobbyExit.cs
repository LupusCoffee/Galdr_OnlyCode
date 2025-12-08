using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class LobbyExit : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private GameObject elevator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SfxManager.Instance.PostEvent("Play_ElevatorJingle");
            SpawnpointManager.SetSpawnpoint(1, 0);
            FirstPersonController.Instance.transform.SetParent(elevator.transform);
            FirstPersonController.Instance.SetCanMove(false);
            cam.enabled = true;
            StartCoroutine(MoveElevatorDown());
            //TransitionAnimator.Instance.LoadGame(1);
        }
    }

    IEnumerator MoveElevatorDown()
    {
        float speed = 0;
        while (elevator.transform.position.y > -200)
        {
            speed += Time.deltaTime * 0.05f;
            elevator.transform.position += Vector3.down * speed;
            yield return null;

            if (elevator.transform.position.y < -130)
            {
                TransitionAnimator.Instance.LoadGame(1);
            }
        }
    }
}
