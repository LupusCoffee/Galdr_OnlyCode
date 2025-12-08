using System;
using UnityEngine;
using UnityEngine.UI;

public class UIInput : MonoBehaviour
{
    [SerializeField] [Tooltip("the image who's sprite we set, based on what controller we are using")]
    private Image imageRef;
    
    [Header("For Automatic Switch")]
    [SerializeField]
    private bool autoSwitch = false; 
    [SerializeField]
    private InputSpellInput spellInput;

    private void Update() //coroutine - only do this ever 0.5 sec
    {
        if (autoSwitch) SetImageAutomatically();
    }

    private void SetImageAutomatically()
    {
        if (IsUsingController()) imageRef.sprite = GetGamepadIconsByInput(spellInput);
        else imageRef.sprite = GetKeybaordIconsByInput(spellInput);
    }
    public void SetImageByInput(InputSpellInput input)
    {
        if (IsUsingController()) imageRef.sprite = GetGamepadIconsByInput(input);
        else imageRef.sprite = GetKeybaordIconsByInput(input);
    }

    private bool IsUsingController()
    {
        return Input.GetJoystickNames().Length > 0;
    }
    
    private Sprite GetGamepadIconsByInput(InputSpellInput input)
    {
        UiManager.GamepadIcons gamepadIcons = UiManager.Instance.GetGamepadIcons();

        switch (input)
        {
            case InputSpellInput.SOUTH:
                return gamepadIcons.buttonSouth;
            case InputSpellInput.EAST:
                return gamepadIcons.buttonEast;
            case InputSpellInput.NORTH:
                return gamepadIcons.buttonNorth;
            case InputSpellInput.WEST:
                return gamepadIcons.buttonWest;
        }

        return null;
    }
    private Sprite GetKeybaordIconsByInput(InputSpellInput input)
    {
        UiManager.KeyboardIcons keyboardIcons = UiManager.Instance.GetKeyboardIcons();
        
        switch (input)
        {
            case InputSpellInput.SOUTH:
                return keyboardIcons.one;
            case InputSpellInput.EAST:
                return keyboardIcons.two;
            case InputSpellInput.NORTH:
                return keyboardIcons.three;
            case InputSpellInput.WEST:
                return keyboardIcons.four;
        }

        return null;
    }
}
