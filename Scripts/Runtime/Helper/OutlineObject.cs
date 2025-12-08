using UnityEngine;

public class OutlineObject : MonoBehaviour
{
    [SerializeField] [Tooltip("Set to LightLayer8 - for awakenable objects")]
    private RenderingLayerMask outlineLayerBlue;
    
    [SerializeField] [Tooltip("Set to LightLayer7 - for possessable objects")]
    private RenderingLayerMask outlineLayerPink;
    
    [SerializeField] [Tooltip("Set to LightLayer6 - for alterable objects")] 
    private RenderingLayerMask outlineLayerGreen;

    private Renderer[] renderers;
    private uint originalLayer;

    private void Start()
    {
        renderers = TryGetComponent<Renderer>(out var meshRenderer)
            ? new[] { meshRenderer }
            : GetComponentsInChildren<Renderer>();
        originalLayer = renderers[0].renderingLayerMask;
    }

    public void DisableAllHighlights()
    {
        foreach (var rend in renderers)
        {
            rend.renderingLayerMask = originalLayer;
        }
    }
    
    public void SetOutlineBlue(bool enable)
    {
        foreach (var rend in renderers)
        {
            rend.renderingLayerMask = enable
                ? originalLayer | outlineLayerBlue
                : originalLayer;
        }
    }

    public void SetOutlinePink(bool enable)
    {
        foreach (var rend in renderers)
        {
            rend.renderingLayerMask = enable
                ? originalLayer | outlineLayerPink
                : originalLayer;
        }
    }

    public void SetOutlineGreen(bool enable)
    {
        foreach (var rend in renderers)
        {
            rend.renderingLayerMask = enable
                ? originalLayer | outlineLayerGreen
                : originalLayer;
        }
    }

    [ContextMenu("SetOutlinePink")]
    public void SetTheOutlinePinkTest()
    {
        SetOutlinePink(true);
    }

    [ContextMenu("SetOutlineGreen")]
    public void SetTheOutlineGreenTest()
    {
        SetOutlineGreen(true);
    }

    //Workaround (need to check if the green outline is active)
    public bool IsGreenActive()
    {
        // If we have no renderers, assume it's not green
        if (renderers == null || renderers.Length == 0)
            return false;

        // Check if the first renderer's renderingLayerMask has the green bit set
        uint currentMask = renderers[0].renderingLayerMask;
        uint greenMask = (uint)outlineLayerGreen;

        // If the green bit is present, we consider the green outline "active"
        return (currentMask & greenMask) != 0;
    }
}
