using SaveSystem;
using System.Threading.Tasks;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuInWorld : MonoBehaviour
{

    [Header("Menu Components")]
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private GameObject keyboardPanel;
    [SerializeField] private GameObject gamepadPanel;
    [SerializeField] private Selectable continueButton;

    [Header("NEW")] 
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject settingCanvas;
    [SerializeField] private GameObject creditsCanvas;
    [SerializeField] private MenuEventSystemhandler menuEventSystemhandlerMainMenuCanvas;
    [SerializeField] private MenuEventSystemhandler menuEventSystemhandlerSettingsCanvas;
    [SerializeField] private MenuEventSystemhandler menuEventSystemhandlerCreditsCanvas;
    [SerializeField] private CinemachineCamera settingsCamera;
    [SerializeField] private CinemachineCamera mainMenuCamera;
    [SerializeField] private CinemachineCamera creditsCamera;
    
    private void Start() {
        CheckIfSaveFileExist();
    }

    private void OnEnable() {
        UserInputs.Instance._backPausMenu.performed += BackToMainMenuInput;
    }


    
    private void OnDisable() {
        UserInputs.Instance._backPausMenu.performed -= BackToMainMenuInput;
    }

    public void GoToSettingsMenu() {
        mainMenuCamera.gameObject.SetActive(false);
        creditsCamera.gameObject.SetActive(false);
        settingsCamera.gameObject.SetActive(true);
        mainMenuCanvas.SetActive(false);
        creditsCanvas.SetActive(false);
        settingCanvas.SetActive(true);
        menuEventSystemhandlerSettingsCanvas.SetFirstSelected();
    }
    
    public void GoCreditsMenu() {
        mainMenuCamera.gameObject.SetActive(false);
        creditsCamera.gameObject.SetActive(true);
        settingsCamera.gameObject.SetActive(false);
        mainMenuCanvas.SetActive(false);
        creditsCanvas.SetActive(true);
        settingCanvas.SetActive(false);
        menuEventSystemhandlerCreditsCanvas.SetFirstSelected();
    }
    
    private void BackToMainMenuInput(InputAction.CallbackContext obj) {
        BackToMainMenu();
    }
    
    public void BackToMainMenu() {
        audioPanel.SetActive(true);
        controlPanel.SetActive(false);
        videoPanel.SetActive(false);
        keyboardPanel.SetActive(false);
        gamepadPanel.SetActive(true);
        
        mainMenuCamera.gameObject.SetActive(true);
        settingsCamera.gameObject.SetActive(false);
        creditsCamera.gameObject.SetActive(false);
        settingCanvas.SetActive(false);
        creditsCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);

        menuEventSystemhandlerMainMenuCanvas.SetFirstSelected();
    }

    public void StartNewGame() {
        AwaitSaveWipes();
    }

    async void AwaitSaveWipes()
    {
        BooSave.WipeAllSaves();
        await Task.Delay(100);
        TransitionAnimator.Instance.LoadGame(4);
    }

    public void Continue() {
        Debug.Log("Continue");
        SaveManager.LoadLatestScene();
    }

    public void QuitGame() {
        Application.Quit();
    }

    private void CheckIfSaveFileExist() {
        continueButton.gameObject.SetActive(BooSave.HasSaveData);
    }
}
