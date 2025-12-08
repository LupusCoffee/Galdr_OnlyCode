//Created by Mohammed (the sex god)

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SfxManager : MonoBehaviour //shoulda inherited from something together with music manager but meh
{
    public static SfxManager Instance;

    [SerializeField] private List<AK.Wwise.Event> generalSfx;
    [SerializeField] private List<AK.Wwise.Event> playerFootstepEvents;
    [SerializeField] private List<AK.Wwise.Event> puzzleSfx;
    [SerializeField] private List<AK.Wwise.Event> effectEvents;
    Dictionary<string, AK.Wwise.Event> sfxDictionary = new Dictionary<string, AK.Wwise.Event>();

    private List<AkSceneUnloadingEvent> eventsToStopOnSceneUnload = new List<AkSceneUnloadingEvent>();


    private void Awake()
    {
        if(Instance == null) Instance = this;
    }
    private void Start()
    {
        SetDictionary();
        SetMasterLowpassNone();
        //gotta find a way to just get all the events straight into the dictionary without the above lists
    }
    private void OnEnable()
    {
        TransitionAnimator.onLoadScene += OnSceneExited;
    }
    private void OnDisable()
    {
        TransitionAnimator.onLoadScene -= OnSceneExited;
    }

    private void SetDictionary()
    {
        foreach (var _event in generalSfx) sfxDictionary.Add(_event.Name, _event);
        foreach (var _event in playerFootstepEvents) sfxDictionary.Add(_event.Name, _event);
        foreach (var _event in puzzleSfx) sfxDictionary.Add(_event.Name, _event);
        foreach (var _event in effectEvents) sfxDictionary.Add(_event.Name, _event);
    }

    #region Event Posting: Regular
    public void PostEvent(string eventName)
    {
        AK.Wwise.Event _event;

        if(sfxDictionary.TryGetValue(eventName, out _event)) _event.Post(gameObject);
    }
    public void PostEvent(AK.Wwise.Event _event)
    {
        if (_event != null) _event.Post(gameObject);
        else Debug.LogError("Sfx Manager: wwise event is null");
    }
    public void PostEvent(string eventName, GameObject objectToPlayOn)
    {
        AK.Wwise.Event _event;

        if (sfxDictionary.TryGetValue(eventName, out _event)) _event.Post(objectToPlayOn);
    }
    public void PostEvent(AK.Wwise.Event _event, GameObject objectToPlayOn)
    {
        if (_event != null) _event.Post(objectToPlayOn);
        else Debug.LogError("Sfx Manager: wwise event is null");
    }
    #endregion

    #region Event Posting: With Delay
    public IEnumerator PostEventWithDelayRoutine(string eventName, GameObject objectToPlayOn, float delay)
    {
        yield return new WaitForSeconds(delay);

        AK.Wwise.Event _event;
        if (sfxDictionary.TryGetValue(eventName, out _event)) _event.Post(objectToPlayOn);

        yield return null;
    }
    public void PostEventWithDelay(string eventName, GameObject objectToPlayOn, float delay)
    {
        StartCoroutine(PostEventWithDelayRoutine(eventName, objectToPlayOn, delay));
    }
    #endregion

    #region Event Posting: Scene Unloading
    public class AkSceneUnloadingEvent
    {
        AK.Wwise.Event startEvent, stopEvent;
        GameObject objectToEmitFrom;

        public AkSceneUnloadingEvent(AK.Wwise.Event _startEvent, AK.Wwise.Event _stopEvent)
        {
            startEvent = _startEvent;
            stopEvent = _stopEvent;
            objectToEmitFrom = Instance.gameObject;
        }
        public AkSceneUnloadingEvent(AK.Wwise.Event _startEvent, AK.Wwise.Event _stopEvent, GameObject _gameObject)
        {
            startEvent = _startEvent;
            stopEvent = _stopEvent;
            objectToEmitFrom = _gameObject;
        }

        public AK.Wwise.Event GetStartEvent() => startEvent;
        public AK.Wwise.Event GetStopEvent() => stopEvent;
        public GameObject GetEmitter() => objectToEmitFrom;
    }
    public void PostStartEventForSceneUnloading(AkSceneUnloadingEvent sound)
    {
        if (sound.GetStartEvent() != null && sound.GetStopEvent() != null)
        {
            sound.GetStartEvent().Post(sound.GetEmitter());
            eventsToStopOnSceneUnload.Add(sound);
        }
        else Debug.LogError("Sfx Manager: wwise event is null");
    }
    public void PostStopEventForSceneUnloading(AkSceneUnloadingEvent sound)
    {
        for (int i = 0; i < eventsToStopOnSceneUnload.Count; i++)
        {
            if(sound == eventsToStopOnSceneUnload[i])
            {
                eventsToStopOnSceneUnload[i].GetStopEvent().Post(eventsToStopOnSceneUnload[i].GetEmitter());
                eventsToStopOnSceneUnload.RemoveAt(i);
            }
        }
    }
    private void OnSceneExited()
    {
        foreach (AkSceneUnloadingEvent sound in eventsToStopOnSceneUnload)
            sound.GetStopEvent().Post(sound.GetEmitter());

        eventsToStopOnSceneUnload.Clear();
    }
    #endregion

    #region Event Posting: Continous
    public IEnumerator PostContinous(AK.Wwise.Event _event, float frequency)
    {
        while(true)
        {
            _event.Post(gameObject);

            yield return new WaitForSeconds(frequency);
        }
        
    }
    public IEnumerator PostContinous(string eventName, float frequency)
    {
        while(true)
        {
            AK.Wwise.Event _event;
            if (sfxDictionary.TryGetValue(eventName, out _event)) _event.Post(gameObject);

            yield return new WaitForSeconds(frequency);
        }
    }
    public IEnumerator PostContinous(AK.Wwise.Event _event, GameObject objectToPlayOn, float frequency)
    {

        while(true)
        {
            _event.Post(objectToPlayOn);

            yield return new WaitForSeconds(frequency);
        }
    }
    public IEnumerator PostContinous(string eventName, GameObject objectToPlayOn, float frequency)
    {
        while(true)
        {
            AK.Wwise.Event _event;
            if (sfxDictionary.TryGetValue(eventName, out _event)) _event.Post(objectToPlayOn);

            yield return new WaitForSeconds(frequency);
        }
    }
    #endregion

    #region Event Posting: TryBool
    public bool TryPostEvent(string eventName)
    {
        AK.Wwise.Event _event;

        if (sfxDictionary.TryGetValue(eventName, out _event))
        {
            _event.Post(gameObject);
            return true;
        }
        else return false;
    }

    public bool TryPostEvent(AK.Wwise.Event _event)
    {
        if (_event != null)
        {
            _event.Post(gameObject);
            return true;
        }
        else return false;
    }

    public bool TryPostEvent(string eventName, GameObject objectToPlayOn)
    {
        AK.Wwise.Event _event;

        if (sfxDictionary.TryGetValue(eventName, out _event))
        {
            _event.Post(objectToPlayOn);
            return true;
        }
        else return false;
    }

    public bool TryPostEvent(AK.Wwise.Event _event, GameObject objectToPlayOn)
    {
        if (_event != null)
        {
            _event.Post(objectToPlayOn);
            return true;
        }
        else return false;
    }
    #endregion


    #region Specific Effects
    public void SetMasterLowpassFull()
    {
        PostEvent("Effects_LowPassMaster_Full");
    }
    public void SetMasterLowpassNone()
    {
        PostEvent("Effects_LowPassMaster_None");
    }
    #endregion

    //Note:
    //- Cutscenes simply take sfx, music, and state setters from wwise as events.
    //- At the start of a cutscene, we set the AudioState to "cutscene", so that only the cutscene audio is heard (and everything else is quiet)
    //- At the end of a cutscene, we set the AudioState back to "gameplay", so gameplay can be heard
}
