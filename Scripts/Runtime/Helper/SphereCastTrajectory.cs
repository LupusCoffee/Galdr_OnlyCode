using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SphereCastTrajectory : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRenderer;
    public Transform hitMarker;             // The "circle" marker placed at collision
    public ThornshellMouth thornshellMouth; // Reference in Inspector to your ThornshellMouth

    [Header("Trajectory Settings")]
    public float throwStrength = 5f;   // Force (impulse) magnitude
    public float timeStep = 0.1f;      // Seconds per simulation step
    public int maxSteps = 30;          // How many steps to simulate
    public float sphereRadius = 0.2f;  // Radius of the thrown object's collider

    private void Awake()
    {
        // Ensure we have a LineRenderer assigned
        if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();

        // Make sure our marker is initially hidden (if we have one)
        if (hitMarker)
        {
            hitMarker.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // 1) Check ThornshellMouth's "active" bool
        if (thornshellMouth && thornshellMouth.active)
        {
            // If active, enable line + marker logic
            if (!lineRenderer.enabled) lineRenderer.enabled = true;

            // Predict and draw the trajectory
            PredictTrajectory();
        }
        else
        {
            // If not active, disable the line and marker
            if (lineRenderer.enabled) lineRenderer.enabled = false;

            if (hitMarker)
                hitMarker.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Predicts the trajectory of a throw using SphereCast to detect collisions,
    /// then places the hitMarker where the projectile would land (if any).
    /// </summary>
    private void PredictTrajectory()
    {
        // 2) INITIAL POSITION / VELOCITY
        Vector3 startPos = transform.position;
        Vector3 direction = (transform.forward + Vector3.up).normalized;
        Vector3 velocity = direction * throwStrength;

        // We'll store the predicted points here
        Vector3[] points = new Vector3[maxSteps];
        points[0] = startPos;

        // Track collision
        bool didHit = false;
        Vector3 hitPoint = Vector3.zero;
        Vector3 hitNormal = Vector3.up;

        int i;
        for (i = 1; i < maxSteps; i++)
        {
            // 3) SIMULATE small time step
            Vector3 prevPos = points[i - 1];
            Vector3 nextPos = prevPos + velocity * timeStep;

            // Apply gravity to velocity
            velocity += Physics.gravity * timeStep;

            // 4) SPHERECAST DETECTION
            Vector3 segmentDir = nextPos - prevPos;
            float dist = segmentDir.magnitude;
            if (dist <= 0f)
            {
                points[i] = nextPos;
                break;
            }

            RaycastHit hit;
            // SphereCast from prevPos to nextPos
            if (Physics.SphereCast(prevPos, sphereRadius, segmentDir.normalized, out hit, dist))
            {
                // We collided with something!
                nextPos = hit.point;
                points[i] = nextPos;

                didHit = true;
                hitPoint = hit.point;
                hitNormal = hit.normal;

                // Stop the loop if we don't want bounces
                i++;
                break;
            }

            // No collision
            points[i] = nextPos;
        }

        // 5) TRIM ARRAY IF STOPPED EARLY
        Vector3[] finalPoints = new Vector3[i];
        for (int j = 0; j < i; j++)
            finalPoints[j] = points[j];

        // 6) DRAW VIA LINERENDERER
        lineRenderer.positionCount = finalPoints.Length;
        lineRenderer.SetPositions(finalPoints);

        // 7) PLACE THE MARKER IF WE HIT
        if (hitMarker)
        {
            if (didHit)
            {
                hitMarker.gameObject.SetActive(true);
                hitMarker.position = hitPoint;
                // Align marker to the surface normal:
                hitMarker.up = hitNormal;
            }
            else
            {
                // No collision -> hide marker or place at last point, up to you
                hitMarker.gameObject.SetActive(false);
            }
        }
    }
}