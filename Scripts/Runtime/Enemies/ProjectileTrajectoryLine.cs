using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ProjectileTrajectoryLine : MonoBehaviour
{
    [Header("Trajectory Settings")]
    public float throwStrength = 5f;
    public int trajectoryResolution = 30;
    public float verticalOffset = 1f;

    // If true, the trajectory line is drawn
    public bool showTrajectory = false;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        // By default, hide the line
        lineRenderer.enabled = false;
        
    }

    private void Update()
    {
        if (showTrajectory)
        {
            RenderTrajectory();
        }
        else
        {
            // Hide the line if not showing
            lineRenderer.enabled = false;
        }
    }

    /// <summary>
    /// Public method to enable/disable drawing the trajectory.
    /// </summary>
    /// <param name="show">True to show trajectory, false to hide.</param>
    public void SetShowTrajectory(bool show)
    {
        showTrajectory = show;

        // If turning on, ensure the line is active
        if (showTrajectory)
            lineRenderer.enabled = true;
    }

    /// <summary>
    /// Renders the projectile path using the set parameters.
    /// </summary>
    private void RenderTrajectory()
    {
        lineRenderer.enabled = true;
        // The direction you want to throw (replace with your own logic if needed)
        Vector3 startPos = transform.position + Vector3.up * verticalOffset;
        Vector3 direction = transform.forward + Vector3.up;
        Vector3 initialVelocity = direction.normalized * throwStrength;

        // Make sure our LineRenderer has enough points
        lineRenderer.positionCount = trajectoryResolution + 1;

        float timestep = 0.1f; // how finely we sample the trajectory
        for (int i = 0; i <= trajectoryResolution; i++)
        {
            float t = timestep * i;
            // Position at time t: S0 + V0 t + (1/2) a t^2
            Vector3 currentPoint = startPos
                + initialVelocity * t
                + 0.5f * Physics.gravity * (t * t);

            lineRenderer.SetPosition(i, currentPoint);
        }
    }
}