using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpellEffectRunTimeData
{
    private SO_SpellPart_Effect spellEffectSO = null;
    private InputSpellInput inputSlot = InputSpellInput.SOUTH;
    private bool isAvailable = false;
    private bool isLocked = true;

    public void Init(SO_SpellPart_Effect _spellEffectSO, InputSpellInput _inputSlot)
    {
        SetSpellEffect(_spellEffectSO);
        SetInputSlot(_inputSlot);
        UpdateUnlockedStatus();
    }
    
    public void SetSpellEffect(SO_SpellPart_Effect _spellEffectSO)
    {
        spellEffectSO = _spellEffectSO;
    }
    public void SetInputSlot(InputSpellInput _inputSlot)
    {
        inputSlot = _inputSlot;
    }
   
    public void UpdateAvailabilityBasedOnMultiTags(List<MultiTag> multiTags)
    {
        foreach (var multiTag in multiTags)
        {
            UpdateAvailabilityBasedOnMultiTag(multiTag);
            if (isAvailable) break;
        }
        
        if(multiTags.Count <= 0) isAvailable = false;
    }
    private void UpdateAvailabilityBasedOnMultiTag(MultiTag multiTag)
    {
        AbilityBehaviour.SpellEffect spellEffectByMultiTag = SpellManager.Instance.GetSpellEffectEnumByMultiTag(multiTag);
        
        if (spellEffectByMultiTag == spellEffectSO.GetSpellEffectEnum()) isAvailable = true;
        else isAvailable = false;
    }
    
    public void UpdateUnlockedStatus()
    {
        //check if unlocked via save data
        //set isUnlocked to whatever it should be
        //always call whenever you pick up a spell effect
        
        isLocked = false;
    }
    
    //Getters
    public bool TryGetSpellEffectSO(out SO_SpellPart_Effect _spellEffectSO)
    {
        _spellEffectSO = spellEffectSO;   
        if (_spellEffectSO == null) return false;
        else return true;
    }
    public InputSpellInput GetInputSpellInput() => inputSlot;
    public bool IsAvailable() => isAvailable;
    public bool IsLocked() => isLocked;
}
