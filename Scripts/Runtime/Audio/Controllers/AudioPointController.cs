using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

public class AudioPointController : MonoBehaviour
{
    [SerializeField] AK.Wwise.Event startPointEvent;
    [SerializeField] AK.Wwise.Event endPointEvent;
    [SerializeField] bool loopEvent;
    [SerializeField] float frequencyMin, frequencyMax;
    [SerializeField] float stopRadius; //to-do: fix it so that if you get out of range, loop stops!


    private void Start()
    {
        if (!loopEvent)
        {
            SfxManager.AkSceneUnloadingEvent soundForUnloading = new SfxManager.AkSceneUnloadingEvent(startPointEvent, endPointEvent, gameObject);
            SfxManager.Instance.PostStartEventForSceneUnloading(soundForUnloading);
        }
        else StartCoroutine(PostEventLoop());
    }

    private IEnumerator PostEventLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(frequencyMin, frequencyMax));

            if (startPointEvent != null) startPointEvent.Post(gameObject);
            else Debug.LogError("AudioPointController: StartPointEvent is null.");

            yield return null;
        }
    }

    //TO-DO: stop coroutine when you're not in range or when you exit scene
    //TO-DO: stop soundForUnloading if not in range
}
