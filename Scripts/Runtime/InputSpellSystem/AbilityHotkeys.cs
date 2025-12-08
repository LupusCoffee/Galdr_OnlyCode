using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityHotkeys : MonoBehaviour
{
    AbilityBehaviour currentAbility = new AbilityBehaviour();

    /*private void OnEnable()
    {
        UserInputs.Instance._attributeLeft.performed += OnNote1;
        UserInputs.Instance._attributeDown.performed += OnNote2;
        UserInputs.Instance._attributeRight.performed += OnNote3;
        UserInputs.Instance._attributeUp.performed += OnNote4;
    }

    private void OnDisable()
    {
        UserInputs.Instance._attributeLeft.performed -= OnNote1;
        UserInputs.Instance._attributeDown.performed -= OnNote2;
        UserInputs.Instance._attributeRight.performed -= OnNote3;
        UserInputs.Instance._attributeUp.performed -= OnNote4;
    }*/

    /*public void OnNote1(InputAction.CallbackContext context)
    {
        print("test");
        currentAbility.Activate(AbilityBehaviour.SpellEffect.ALTER, AbilityBehaviour.SpellTarget.NATURE);
    }
    public void OnNote2(InputAction.CallbackContext obj) { currentAbility.Activate(AbilityBehaviour.SpellEffect.AWAKEN, AbilityBehaviour.SpellTarget.MACHINA); }
    public void OnNote3(InputAction.CallbackContext obj) { currentAbility.Activate(AbilityBehaviour.SpellEffect.POSSESS, AbilityBehaviour.SpellTarget.CORRUPTION); }
    public void OnNote4(InputAction.CallbackContext obj) { currentAbility.Activate(AbilityBehaviour.SpellEffect.AWAKEN, AbilityBehaviour.SpellTarget.NATURE); }
    */

}
