using UnityEngine;

public class ElevatorPulleyConnector : MonoBehaviour
{
    [SerializeField] private Transform pulley;

    [SerializeField] private Transform rope1Start;
    [SerializeField] private Transform rope1End;

    [SerializeField] private Transform rope2Start;
    [SerializeField] private Transform rope2End;

    [SerializeField] private LineRenderer rope1;
    [SerializeField] private LineRenderer rope2;



    void Update()
    {
        rope1.SetPosition(0, rope1Start.position);
        rope1.SetPosition(1, rope1End.position);

        rope2.SetPosition(0, rope2Start.position);
        rope2.SetPosition(1, rope2End.position);
    }
}
