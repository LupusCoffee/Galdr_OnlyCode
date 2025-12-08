using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour //shoulda inherited from something together with sfx manager but meh
{
    public static MusicManager Instance;

    [Header("Background Music")]
    [SerializeField] private List<AK.Wwise.Event> levelMusicEvents;
    [SerializeField] private List<AK.Wwise.Event> cutsceneMusicEvents;
    [SerializeField] private List<AK.Wwise.Event> puzzleMusicEvents;
    [SerializeField] private List<AK.Wwise.Event> miscMusicEvents;

    /*[Header("Technical Shit - i think we dont use it anymore?")]
    [SerializeField] private List<AK.Wwise.Event> baseEventScale;
    [SerializeField] private List<AK.Wwise.Event> lightEventScale;
    [SerializeField] private List<AK.Wwise.Event> mindEventScale;
    private List<AK.Wwise.Event> currentEventScale;*/

    Dictionary<string, AK.Wwise.Event> bgMusicDictionary = new Dictionary<string, AK.Wwise.Event>();
    
    public enum MusicScales{ BASE, LIGHT, MIND }
    MusicScales currentScale;

    private enum MusicScene{ MAIN_MENU, EXPLORATION, LEVEL1, LEVEL2 }
    Scene currentScene;
    bool sceneMusicPlaying;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        SetDictionary();

        //currentEventScale = baseEventScale;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        TransitionAnimator.onLoadScene += OnSceneExited;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        TransitionAnimator.onLoadScene -= OnSceneExited;
    } 


    private void SetDictionary()
    {
        foreach (var _event in levelMusicEvents) bgMusicDictionary.Add(_event.Name, _event);
        foreach (var _event in cutsceneMusicEvents) bgMusicDictionary.Add(_event.Name, _event);
        foreach (var _event in puzzleMusicEvents) bgMusicDictionary.Add(_event.Name, _event);
        foreach (var _event in miscMusicEvents) bgMusicDictionary.Add(_event.Name, _event);
    }

    public void PostEvent(string eventName)
    {
        AK.Wwise.Event _event;
        if (bgMusicDictionary.TryGetValue(eventName, out _event)) _event.Post(gameObject);
        else Debug.LogError("Music Manager: no wwise event with the name '" + eventName + "' could be found.");
    }
    public void PostEvent(AK.Wwise.Event _event)
    {
        if (_event != null) _event.Post(gameObject);
        else Debug.LogError("Music Manager: wwise event is null");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene; //miiight be needed for exiting the scene and stopping the music early for fading

        switch (currentScene.buildIndex)
        {
            case (int)MusicScene.MAIN_MENU:
                PostEvent("Music_MainMenu_Play");
                break;

            case (int)MusicScene.EXPLORATION:
                PostEvent("Music_Exploration_Play");
                break;

            case (int)MusicScene.LEVEL1:
                PostEvent("Music_ForestLevel1_Play");
                break;

            case (int)MusicScene.LEVEL2:
                PostEvent("Music_ForestLevel2_Play");
                break;

            default:
                Debug.LogWarning("MusicManager: Scene does not have a music track to play.");
                break;
        }
    }
    private void OnSceneExited() //TO-DO: let the song fade out before the next scene starts
    {
        switch (currentScene.buildIndex)
        {
            case (int)MusicScene.MAIN_MENU:
                PostEvent("Music_MainMenu_Stop");
                break;

            case (int)MusicScene.EXPLORATION:
                PostEvent("Music_Exploration_Stop");
                break;

            case (int)MusicScene.LEVEL1:
                PostEvent("Music_ForestLevel1_Stop");
                break;

            case (int)MusicScene.LEVEL2:
                PostEvent("Music_ForestLevel2_Stop");
                break;

            default:
                break;
        }
    }


    //Setters
    public void SetCurrentScale(MusicScales scale)
    {
        currentScale = scale;

        /*switch (scale)
        {
            case MusicScales.BASE: currentEventScale = baseEventScale;
                break;

            case MusicScales.LIGHT: currentEventScale = lightEventScale;
                break;

            case MusicScales.MIND: currentEventScale = mindEventScale;
                break;
        }*/
    }
    public void ResetCurrentScale()
    { 
        currentScale = MusicScales.BASE; 
        //currentEventScale = baseEventScale; 
    }

    //Getters
    //public List<AK.Wwise.Event> GetCurrentScale() => currentEventScale;



    //Note:
    //- Cutscenes simply take sfx, music, and state setters from wwise as events.
    //- At the start of a cutscene, we set the AudioState to "cutscene", so that only the cutscene audio is heard (and everything else is quiet)
    //- At the end of a cutscene, we set the AudioState back to "gameplay", so gameplay can be heard
}
