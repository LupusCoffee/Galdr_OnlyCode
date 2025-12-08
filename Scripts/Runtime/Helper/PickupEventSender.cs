using UnityEngine;

public class PickupEventSender : MonoBehaviour
{
    public delegate void PickupEvent();
    public static event PickupEvent OnPickup;

    private void OnDestroy()
    {
        OnPickup?.Invoke();
    }
}
