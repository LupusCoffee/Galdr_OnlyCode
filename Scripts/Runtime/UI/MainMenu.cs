using SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Encrypted Text Setup")]
    [SerializeField] TMP_SpriteAsset encryptedSymbolsAsset;

    [SerializeField] private MenuEventSystemhandler menuEventSystemhandler;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject keyboardPanel;
    [SerializeField] private GameObject gamepadPanel;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private Selectable continueButton;

    private void Awake() {
        BooSave.Shared.Clear();
        BooSave.WipeSaveByName("inventory.dat");

        LanguageEncrypter.SetUnknownSymbols(encryptedSymbolsAsset);
    }

    private void Start() {
        CheckIfSaveFileExist();
    }

    private void OnEnable() {
        UserInputs.Instance._backPausMenu.performed += BackToMainMenuInput;
    }


    
    private void OnDisable() {
        UserInputs.Instance._backPausMenu.performed -= BackToMainMenuInput;
    }
    
    private void BackToMainMenuInput(InputAction.CallbackContext obj) {
        audioPanel.SetActive(true);
        controlPanel.SetActive(false);
        videoPanel.SetActive(false);
        settingsPanel.SetActive(false);
        keyboardPanel.SetActive(false);
        gamepadPanel.SetActive(true);
        startPanel.SetActive(true);
        menuEventSystemhandler.SetFirstSelected();
    }

    public void StartNewGame() {
        TransitionAnimator.Instance.LoadGame(1);
    }

    public void Continue() {
        Debug.Log("Continue");
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void CheckIfSaveFileExist() {
        if (BooSave.Shared.IsEmpty) {
            continueButton.gameObject.SetActive(false);
        }
        else {
            continueButton.gameObject.SetActive(true);
        }
    }
}
