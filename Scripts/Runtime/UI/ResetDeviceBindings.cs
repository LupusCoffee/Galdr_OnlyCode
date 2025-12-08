using UnityEngine;
using UnityEngine.InputSystem;

public class ResetDeviceBindings : MonoBehaviour {
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private string _targetControlScheme;

    public void ResetControlSchemeBindings() {
        foreach (InputActionMap map in _inputActions.actionMaps) {
            foreach (InputAction action in map.actions) {
                action.RemoveBindingOverride(InputBinding.MaskByGroup(_targetControlScheme));
            }
        }
    }
}
