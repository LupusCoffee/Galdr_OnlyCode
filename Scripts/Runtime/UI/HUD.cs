using System;
using UnityEngine;
using UnityEngine.Events;

public class HUD : MonoBehaviour {
    public static HUD Instance;
    
    public UnityEvent _updatePromptButton;
    
    [Header("NoteSheet")]
    public GameObject noteSheetKeyKeyboard;
    public GameObject noteSheetKeyGamepad;
    
    [Header("Journal")]
    public GameObject journalKeyKeyboard;
    public GameObject journalKeyGamepad;

    public GameObject hudPanel;
    
    private ControllerHelper _controllerHelper;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start() {
        _controllerHelper = FindFirstObjectByType<ControllerHelper>();
        UpdatePrompt();
        _updatePromptButton?.Invoke();
    }

    public void UpdatePrompt() {
        if (_controllerHelper != null) {
            if (_controllerHelper.isSwitchController || _controllerHelper.isXboxController ||
                _controllerHelper.isPSController) {
                noteSheetKeyGamepad.SetActive(true);
                journalKeyGamepad.SetActive(true);
                noteSheetKeyKeyboard.SetActive(false);
                journalKeyKeyboard.SetActive(false);
            }
            else {
                noteSheetKeyGamepad.SetActive(false);
                journalKeyGamepad.SetActive(false);
                noteSheetKeyKeyboard.SetActive(true);
                journalKeyKeyboard.SetActive(true);
            }
        }
        _updatePromptButton?.Invoke();
    }

    public void HideHUD() {
        hudPanel.SetActive(false);
    }
    
    public void ShowHUD() {
        hudPanel.SetActive(true);
    }
}
