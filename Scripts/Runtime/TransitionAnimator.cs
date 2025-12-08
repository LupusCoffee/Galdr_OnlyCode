using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static TransitionAnimator;

[DefaultExecutionOrder(-100)]
public class TransitionAnimator : MonoBehaviour
{
    public static TransitionAnimator Instance;

    public delegate void OnLoadScene();
    public static event OnLoadScene onLoadScene;

    Vector3 levelSpawnZone = new Vector3(0, 0, 0);
    bool isResetting;

    [SerializeField] GameObject throbber;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        fadeImage.SetActive(true);
    }

    [SerializeField] GameObject fadeImage;
    public Animator transition;
    public bool isTransitioning;

    public void LoadGame(int levelIndex)
    {
        if (isTransitioning)
        {
            return;
        }

        isTransitioning = true;
        StartCoroutine(LoadLevel(levelIndex));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        throbber.SetActive(true);
        onLoadScene?.Invoke();

        transition.SetTrigger("START");
        yield return new WaitForSeconds(1);

        AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }

        // Uodate the progression manager
        ProgressionManager.Instance.OnSceneLoad();

        yield return new WaitForSeconds(0.5f);
        isTransitioning = false;
        FadeIn();
    }

    public void FadeOut()
    {
        
        transition.SetTrigger("START");
        
    }

    public void FadeIn()
    {
        
        transition.SetTrigger("END");
        throbber.SetActive(false);

    }

    public void ResetLevel()
    {
        if (isResetting) {
            return;
        }

        StartCoroutine(ResetLevelFade());
    }
    
    IEnumerator ResetLevelFade()
    {

        isResetting = true;
        GameState.Pause();
        FadeOut();

        yield return new WaitForSeconds(0.75F);

        // tp back to spawnpoint
        // if (newPos != Vector3.zero)
        // {
        //     Player.Instance.transform.position = newPos;
        // }
        // else
        // {
        //     Player.Instance.transform.position = levelSpawnZone;
        // }

        Player.Instance.ResetPlayerToSave();

        yield return new WaitForSeconds(0.75F);

        FadeIn();
        GameState.Unpause();
        isResetting = false;
    }

    public void SetLevelSpawnZone(Vector3 pos)
    {
        levelSpawnZone = pos;
    }
}
