using SaveSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class ProgressionManager : MonoBehaviour
{
    public static ProgressionManager Instance;

    [Header("Stage info:" +
        "\nLevel 0 = Main Menu" +
        "\nLevel 1 = Exploration Scene" +
        "\n Level 2 = Alex Scene" +
        "\n Level 4 = Zonghao Scene" +
        "\n Level 4 = Lobby Scene" +
        "\n Level 5 = River Cutscene")]
    [SerializeField] private Stage[] stages;

    GameObject mustHaveComponents;

    public enum CurrentStage
    {
        Intro, // play the intro cutscenes such as the fall and the river
        LobbyCutscene, // cutscene where the player is introduced to the lobby
        SearchingForPossessSpell, // player is searching for the possess spell
        UnlockingPossessSpell, // player is unlocking the possess spell
        Freeplay // player is free to explore the place
    }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(this); }
    }

    void Start()
    {
        if (BooSave.Shared.ContainsKey("currentStage"))
        {
            currentStage = (CurrentStage)BooSave.Shared.Load<long>("currentStage");
        }
        else
        {
            currentStage = CurrentStage.Intro;
        }
    }

    public CurrentStage currentStage = CurrentStage.Intro;

    public void SetCurrentStage(CurrentStage stage)
    {
        currentStage = stage;
        BooSave.Shared.Save((int)currentStage, "currentStage");

        EnablePlayerControls();
    }

    public void OnSceneLoad()
    {
        if ((IsLobbyScene() && currentStage == CurrentStage.Intro) || (IsLobbyScene() && currentStage == CurrentStage.LobbyCutscene))
        {
            if(FindFirstObjectByType<UserInputs>().gameObject != null)
            {
                mustHaveComponents = FindFirstObjectByType<UserInputs>().gameObject;
                mustHaveComponents.SetActive(false);
            }
        }

        foreach (Stage stage in stages)
        {
            if (stage.associatedStage == currentStage)
            {
                foreach (StageSpawns stageSpawn in stage.stageSpawns)
                {
                    if (stageSpawn.sceneIndex == SceneManager.GetActiveScene().buildIndex)
                    {
                        //if there is no cutscene, skip
                        if (stageSpawn.cutScene == null)
                        {
                            Debug.Log("no cutscene for stage " + currentStage);
                            return;
                        }
                        
                        Debug.Log("Playing cutscene for stage: " + currentStage);

                        if (ShouldDisablePlayerControls())
                        {
                            DisablePlayerControls();
                        }

                        GameObject cutsceneObject = Instantiate(stageSpawn.cutScene);

                        PlayableDirector director = cutsceneObject.GetComponent<PlayableDirector>();
                        director.stopped += WaitForTimelineToEnd;
                    }
                }
            }
        }
    }

     
    void WaitForTimelineToEnd(PlayableDirector aDirector)
    {
        if (currentStage == CurrentStage.Intro && IsLobbyScene())
        {
            Destroy(aDirector.gameObject);

            Debug.Log("Intro cutscene ended");
            TransitionAnimator.Instance.LoadGame(5);
        }
        else if (currentStage == CurrentStage.LobbyCutscene && IsLobbyScene())
        {
            Destroy(aDirector.gameObject);
            currentStage = CurrentStage.SearchingForPossessSpell;
            BooSave.Shared.Save((int)currentStage, "currentStage");

            mustHaveComponents.SetActive(true);

            EnablePlayerControls();
        }
        else if (currentStage == CurrentStage.UnlockingPossessSpell && IsLobbyScene())
        {
            Destroy(aDirector.gameObject);
            currentStage = CurrentStage.Freeplay;
            BooSave.Shared.Save((int)currentStage, "currentStage");

            Inventory.TryUnlockItem(67108865); // possess spell
            Inventory.TryUnlockItem(67108867); // corruption spell


            EnablePlayerControls();
        }
    }

    bool IsIntroScene()
    {
        return SceneManager.GetActiveScene().buildIndex == 0;
    }

    bool IsLobbyScene()
    {
        return SceneManager.GetActiveScene().buildIndex == 4;
    }

    bool IsRiverScene()
    {
        return SceneManager.GetActiveScene().buildIndex == 5;
    }

    bool IsExplorationScene()
    {
        return SceneManager.GetActiveScene().buildIndex == 1;
    }

    bool IsAlexScene()
    {
        return SceneManager.GetActiveScene().buildIndex == 2;
    }

    bool ShouldDisablePlayerControls()
    {
        return currentStage == CurrentStage.Intro || currentStage == CurrentStage.LobbyCutscene || currentStage == CurrentStage.UnlockingPossessSpell;
    }

    void DisablePlayerControls()
    {
        if (Player.Instance != null)
            Player.Instance.FullyDisablePlayerControls();
        else if (FirstPersonController.Instance != null)
            FirstPersonController.Instance.SetCanMove(false);
    }

    void EnablePlayerControls()
    {
        if (Player.Instance != null)
            Player.Instance.FullyEnablePlayerControls();
        
        if (FirstPersonController.Instance != null)
            FirstPersonController.Instance.SetCanMove(true);
    }

    [System.Serializable]
    public class Stage
    {
        [SerializeField] public CurrentStage associatedStage;
        [SerializeField] public List<StageSpawns> stageSpawns;
    }

    [System.Serializable]
    public class StageSpawns
    {
        [SerializeField] public int sceneIndex;
        [SerializeField] public GameObject cutScene;
    }
}

