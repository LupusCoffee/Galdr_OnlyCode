// Made by Isabelle H. Heiskanen

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerHelper : MonoBehaviour {
    public bool isGamepadPluggedIn;
    public bool isSwitchController;
    public bool isPSController;
    public bool isXboxController;


    private void OnEnable() {
        InputSystem.onDeviceChange += OnDeviceChanged;
    }

    private void OnDisable() {
        InputSystem.onDeviceChange -= OnDeviceChanged;
    }

    private void Start() {
        SetController();
        StartCoroutine(DelayForButton());
    }

    private void SetController() {
        if (Gamepad.current == null) {
            isGamepadPluggedIn = false;
            isSwitchController = false;
            isXboxController = false;
            isPSController = false;
            Debug.Log("No controller");
        }

        if (Gamepad.current != null && Gamepad.current.name == "DualShock4GamepadHID") {
            Debug.Log("Playstation controller");
            isSwitchController = false;
            isPSController = true;
            isXboxController = false;
        }

        if (Gamepad.current != null && Gamepad.current.name == "XInputControllerWindows") {
            Debug.Log("Xbox controller");
            isSwitchController = false;
            isPSController = false;
            isXboxController = true;
        }

        if (Gamepad.current != null && Gamepad.current.name == "SwitchProControllerHID") {
            Debug.Log("Switch controller");
            isSwitchController = true;
            isPSController = false;
            isXboxController = false;
        }
        
        HUD.Instance.UpdatePrompt();
    }


    private void OnDeviceChanged(InputDevice device, InputDeviceChange change) {
        switch (change) {
            case InputDeviceChange.Added:
                Debug.Log($"Device {device} was added");
                SetController();
                break;
            case InputDeviceChange.Removed:
                Debug.Log($"Device {device} was removed");
                SetController();
                break;
        }
        HUD.Instance.UpdatePrompt();
    }

    private IEnumerator DelayForButton() {
        yield return new WaitForSeconds(0.5f);
        HUD.Instance.UpdatePrompt();
    }
}