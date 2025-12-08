using UnityEngine;
using static CompactMath;

public class Heatseeker : Enemy
{
    [SerializeField] float speed = .1f;
    [SerializeField] float rotationSpeed = 200.0f;

    [SerializeField] LayerMask targetMask;

    Transform target;

    float maxDistanceFromSpawn = 10;
    bool chasingPlayer;
    Vector3 spawnPos;

    Vector3 currentTarget;

    float consumptionTime = 2;
    float consumptionTimer = 0;

    private void Awake()
    {
        spawnPos = transform.position;
        currentTarget = RandomPosInRange(spawnPos, maxDistanceFromSpawn, null, 2);

        dazeDuration = 0.5f;
    }

    protected override void Start()
    {
        base.Start();
        target = Player.Instance.gameObject.transform;
    }

    void Update()
    {
        if (IsPaused || dazed) {
            return;
        }

        chasingPlayer = Vector3.Distance(spawnPos, target.position) < maxDistanceFromSpawn;

        if (chasingPlayer)
        {
            currentTarget = target.position;
        }
        float currentSpeed = chasingPlayer ? speed * 2 : speed;

        if (chasingPlayer)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            transform.position += transform.forward * currentSpeed * Time.deltaTime;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget, currentSpeed * Time.deltaTime);
            if(Vector3.Distance(transform.position, currentTarget) < 0.1f) {
                currentTarget = RandomPosInRange(spawnPos, maxDistanceFromSpawn, null, 2);
            }
        }
    }
}
