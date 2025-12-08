using UnityEngine;
using UnityEngine.AI;
using static CompactMath;

public class ThornshellBrain : PausableMonoBehaviour
{
    [SerializeField] float speed;
    MindControl mindControl;

    bool isInView = false;
    bool isOnButton = false;

    Rigidbody rb;

    Vector3 spawnPos;

    private void Awake()
    {
        mindControl = GetComponent<MindControl>();
        rb = GetComponent<Rigidbody>();

        spawnPos = transform.position;

        InvokeRepeating("IsOnButton", 0, 1);
    }



    private void Update()
    {
        isInView = ViewChecker();

        if (mindControl.isMindControlled || isInView || isOnButton || Player.Instance.isOutOfBody) {
            return;
        }

        if(Vector3.Distance(spawnPos, transform.position) < 1f) {
            return;
        }

        Vector3 direction = (spawnPos - transform.position).normalized;

        //check so lookrotation is not zero
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, ScaledDeltaTime);
        }

        transform.position += transform.forward * speed * ScaledDeltaTime;
    }

    private bool ViewChecker()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        bool onScreen = screenPoint.z > 0f && screenPoint.x > 0f && screenPoint.x < 1f && screenPoint.y > 0f && screenPoint.y < 1f;
        return onScreen;
    }

    private void IsOnButton()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f);

        foreach (Collider col in colliders) {
            if (col.gameObject.GetComponent<Puzzle_PressurePlate>() != null)
            {
                isOnButton = true;
                return;
            }
        }

        isOnButton = false;
    }
}
