using UnityEngine;

public class AudioButtons : MonoBehaviour
{
    public void PressButton()
    {
        SfxManager.Instance.PostEvent("PressUi");
    }

    public void HoverButton()
    {
        SfxManager.Instance.PostEvent("HoverUi");
    }
}
