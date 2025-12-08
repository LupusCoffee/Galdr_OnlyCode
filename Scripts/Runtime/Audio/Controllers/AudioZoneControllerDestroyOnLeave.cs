//Created by Mohammed (the sex god)

using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioZoneController1 : MonoBehaviour
{
    [SerializeField] bool playWithoutPlayer;
    [SerializeField] AK.Wwise.Event enterEvent, exitEvent;
    SfxManager.AkSceneUnloadingEvent soundForUnloading;

    private void Start()
    {
        soundForUnloading = new SfxManager.AkSceneUnloadingEvent(enterEvent, exitEvent);

        if (playWithoutPlayer) EnterZone();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) EnterZone();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) ExitZone();
    }


    protected virtual void EnterZone()
    {
        SfxManager.Instance.PostStartEventForSceneUnloading(soundForUnloading);
    }
    protected virtual void ExitZone()
    {
        SfxManager.Instance.PostStopEventForSceneUnloading(soundForUnloading);
    }
}