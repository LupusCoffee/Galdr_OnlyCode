using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoteSheetBindings : MonoBehaviour
{
    public GameObject interactKeyboard2;
    public GameObject interactGamepad2;
    public GameObject interactKeyboard1;
    public GameObject interactGamepad1;
    
    private ControllerHelper _controllerHelper;
    private InputSpellController _flutePlayer;
    
    private void Start() {
        _controllerHelper = FindFirstObjectByType<ControllerHelper>();
        _flutePlayer = FindFirstObjectByType<InputSpellController>();
        
        ShowCorrectButton();
    }

    private void Update() {
        if (!_flutePlayer.SpellControllerActive) return;
        
        ShowCorrectButton();
    }

    private void ShowCorrectButton() {
        if (_controllerHelper != null) {
            if (_controllerHelper.isSwitchController || _controllerHelper.isXboxController ||
                _controllerHelper.isPSController) {
                interactGamepad2.SetActive(true);
                interactGamepad1.SetActive(true);
                interactKeyboard2.SetActive(false);
                interactKeyboard1.SetActive(false);
            }
            else {
                interactGamepad2.SetActive(false);
                interactGamepad1.SetActive(false);
                interactKeyboard2.SetActive(true);
                interactKeyboard1.SetActive(true);
            }
        }
    }
}
