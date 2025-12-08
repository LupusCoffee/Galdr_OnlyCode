using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using static CompactMath;

public class Awaken_Rotate : Awakenable
{
    public enum AxisRotation
    {
        X,
        Y,
        Z
    }

    [Header("General Stats")]
    [SerializeField] private AxisRotation axisRotation = AxisRotation.Y;

    [Header("Cycles Awakening")]
    [SerializeField] private int cycleAngle = 90; // angle to rotate in one cycle
    [SerializeField] private int numberOfCycles = 1;
    [SerializeField] private float timePerCycle = 5f; // time to make it full cycle
    [SerializeField] private float pauseTime = 1f; // time to pause between cycles


    private Vector3 startRot = Vector3.zero;

    private int currentCycle = 0;

    private void Awake()
    {
        startRot = transform.eulerAngles;
    }

    private async void CycleMove()
    {
        float currentAngle = transform.eulerAngles.y;

        for (int i = 0; i < numberOfCycles; i++)
        {
            float elapsedTime = 0f;

            while (elapsedTime < timePerCycle)
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / timePerCycle);

                float angle = currentAngle + cycleAngle * progress;

                Vector3 rot = startRot;
                switch (axisRotation)
                {
                    case AxisRotation.X:
                        rot.x = angle % 360f; // Ensure angle stays within 0-360 degrees
                        break;
                    case AxisRotation.Y:
                        rot.y = angle % 360f;
                        break;
                    case AxisRotation.Z:
                        rot.z = angle % 360f;
                        break;
                }

                transform.eulerAngles = rot;
                await Task.Yield();
            }

            currentAngle += cycleAngle; // Move to the next cycle's start angle
            await Task.Delay(SecondsToMilli(pauseTime));
        }

        isAwakened = false;
    }


    public override void SpellHit()
    {
        if (isAwakened) {
            return;
        }

        isAwakened = true;
        CycleMove();
    }
}
