using UnityEngine;

public class PuzzleEvent : MonoBehaviour
{
    public delegate void OnPress(GameObject caller);
    public event OnPress OnButtonPressed;
    public event OnPress OnButtonReleased;

    protected void TriggerOnButtonPressed()
    {
        OnButtonPressed?.Invoke(gameObject);
    }

    protected void TriggerOnButtonReleased()
    {
        OnButtonReleased?.Invoke(gameObject);
    }

    private void OnDestroy()
    {
        OnButtonPressed = null;
        OnButtonReleased = null;
    }

    // public access methods for button press and release
    public void ButtonPress()
    {
        TriggerOnButtonPressed();
    }

    public void ButtonRelease()
    {
        TriggerOnButtonReleased();
    }
}
