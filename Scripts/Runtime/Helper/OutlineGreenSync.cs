using UnityEngine;

public class OutlineGreenSync : MonoBehaviour
{
    [Header("The OutlineObject we want to check")]
    [SerializeField] private OutlineObject source;

    [Header("The OutlineObject we want to turn green if 'source' is green")]
    [SerializeField] private OutlineObject target;

    private void Update()
    {
        // Safety checks
        if (source == null || target == null) return;

        // Check if the source OutlineObject is currently green
        bool sourceGreen = source.IsGreenActive();

        // Make the target the same green state
        target.SetOutlineGreen(sourceGreen);
    }
}