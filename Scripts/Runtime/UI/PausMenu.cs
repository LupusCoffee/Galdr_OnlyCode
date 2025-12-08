using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PausMenu : MonoBehaviour {
    
    [SerializeField] private CanvasGroup pausMenuCanvasGroup;
    [SerializeField] private MenuEventSystemhandler menuEventSystemhandler;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject keyboardPanel;
    [SerializeField] private GameObject gamepadPanel;
    [SerializeField] private GameObject startPanel;
    
    
    private float _desiredAlpha;
    private float _currentAlpha;
    private bool _isPausMenuOpen;

    private void OnEnable() {
        UserInputs.Instance._pausMenu.performed += PausInput;
        UserInputs.Instance._backPausMenu.performed += BackToPausMenuInput;
    }



    private void OnDisable() {
        UserInputs.Instance._pausMenu.performed -= PausInput;
        UserInputs.Instance._backPausMenu.performed -= BackToPausMenuInput;
    }

    private void Awake() {
    
        _currentAlpha = 0;
        _desiredAlpha = 0;
        pausMenuCanvasGroup.interactable = false;
        pausMenuCanvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Fades the journal in and out
    /// </summary>
    private void Update() {
        _currentAlpha = Mathf.MoveTowards(_currentAlpha, _desiredAlpha, 2.0f * Time.deltaTime);
        pausMenuCanvasGroup.alpha = _currentAlpha;
    }
    
    
    private void PausInput(InputAction.CallbackContext obj) {
        if (_isPausMenuOpen) {
            ClosePausMenu();
        }
        else if (!_isPausMenuOpen) {
            OpenPausMenu();
        }
    }
    
    private void BackToPausMenuInput(InputAction.CallbackContext obj) {
     BackToPausMenu();
    }
    
    public void BackToPausMenu() {
        audioPanel.SetActive(true);
        controlPanel.SetActive(false);
        videoPanel.SetActive(false);
        settingsPanel.SetActive(false);
        keyboardPanel.SetActive(false);
        gamepadPanel.SetActive(true);
        startPanel.SetActive(true);
        menuEventSystemhandler.SetFirstSelected();
    }

    private void OpenPausMenu() {
        startPanel.SetActive(true);
        _desiredAlpha = 1;
        pausMenuCanvasGroup.interactable = true;
        pausMenuCanvasGroup.blocksRaycasts = true;
        UserInputs.Instance.OpenPausMenu();
        _isPausMenuOpen = true;
        menuEventSystemhandler.SetFirstSelected();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameState.Pause();
    }

    public void ClosePausMenu() {
        _desiredAlpha = 0;
        pausMenuCanvasGroup.interactable = false;
        pausMenuCanvasGroup.blocksRaycasts = false;
        UserInputs.Instance.ClosePausMenu();
        _isPausMenuOpen = false;
        
        audioPanel.SetActive(true);
        controlPanel.SetActive(false);
        videoPanel.SetActive(false);
        settingsPanel.SetActive(false);
        keyboardPanel.SetActive(false);
        gamepadPanel.SetActive(true);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameState.Unpause();
    }

    public void ResetLevel() {
        Scene scene = SceneManager.GetActiveScene();
        TransitionAnimator.Instance.LoadGame(scene.buildIndex);
    }

    public void QuitGame() {
        Application.Quit();
    }
    
}
