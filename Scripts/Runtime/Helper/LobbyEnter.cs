using System.Collections;
using UnityEngine;

public class LobbyEnter : MonoBehaviour
{
    [SerializeField] GameObject elevatorCam;
    [SerializeField] GameObject elevator;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            elevatorCam.SetActive(true);
            //other.gameObject.SetActive(false);

            Player.Instance.GetComponent<Collider>().enabled = false;
            Player.Instance.GetComponent<Rigidbody>().isKinematic = true;
            Player.Instance.transform.SetParent(elevator.transform);
            Player.Instance.transform.position = elevator.transform.position + Vector3.up * 1.25f;
            Player.Instance.DisableControls(null);

            StartCoroutine(MoveElevatorUp());
        }
    }

    IEnumerator MoveElevatorUp()
    {
        float speed = 0;
        while (elevator.transform.position.y < 1000)
        {
            speed += Time.deltaTime * 0.1f;
            elevator.transform.position += Vector3.up * speed;
            yield return null;

            if (elevator.transform.position.y > 100)
            {
                TransitionAnimator.Instance.LoadGame(4);
            }
        }
    }
}
