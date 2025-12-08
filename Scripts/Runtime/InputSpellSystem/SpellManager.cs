// made by Rolf Mohammed Diab

//note: 
//  I would have liked to set this up for some sort of equip system for the spell effects.
//  However, since the backend only accounts for unlocking spell effects in four set slots,
//  and since there are no plans for making a system for equipping spell effects, I felt it
//  would just be clunky having to both unlock and call an equip function when you get a spell
//  effect. I also did not want to waste time overengineering. The spell targets are expandable
//  beyond four at the very least.
//todo: edit the save system to allow for a system of spell effects equipping.


using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class SpellManager : MonoBehaviour {
    //Variables
    public static SpellManager Instance;
    
    [SerializeField] [Tooltip("0: South Input, 1: East Input, 2: North Input, 3: West Input.")]
    private SO_SpellPart_Effect[] spellEffectSlots = new SO_SpellPart_Effect[Enum.GetValues(typeof(InputSpellInput)).Length];
    
    [SerializeField] //might exist in martin's system already? todo: make it so regardless! and move this!!
    private List<SO_SpellPart_Target> gameUsableSpellTargetsList = new();
    
    private Dictionary<InputSpellInput, SpellEffectRunTimeData> spellEffectRunTimeDataByInput = new();
    private Dictionary<AbilityBehaviour.SpellEffect, SpellEffectRunTimeData> spellEffectRunTimeDataBySpellEffectEnum = new();
    
    private readonly Dictionary<MultiTag.MultiTags, SO_SpellPart_Target> gameUsableSpellTargetsMap = new();
    
    
    //General
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    
    private void Start()
    {
        MultiTag.MultiTags tags;
        
        for (int i = 0; i < spellEffectSlots.Length; i++)
        {
            if (spellEffectSlots[i] == null) continue;
            
            SO_SpellPart_Effect spellEffectSO = spellEffectSlots[i];
            InputSpellInput inputSlot = (InputSpellInput)i;
            
            SpellEffectRunTimeData spellEffectRunTimeData = new();
            spellEffectRunTimeData.Init(spellEffectSO, inputSlot);
            
            spellEffectRunTimeDataByInput.TryAdd((InputSpellInput)i, spellEffectRunTimeData);
            spellEffectRunTimeDataBySpellEffectEnum.TryAdd(spellEffectSO.GetSpellEffectEnum(), spellEffectRunTimeData);
        }
        
        foreach (var target in gameUsableSpellTargetsList)
        {
            tags = GetMultiTagsByTarget(target.GetSpellTargetEnum());
            if (tags == MultiTag.MultiTags.None) continue;
            
            gameUsableSpellTargetsMap.TryAdd(tags, target);
        }
    }

    
    //Getters
    #region Getters
    //todo: these should be somewhere else - some kinda helper script
    public AbilityBehaviour.SpellEffect GetSpellEffectEnumByMultiTag(MultiTag multiTag)
    {
        if (multiTag.HasTag(MultiTag.MultiTags.Alterable))
            return AbilityBehaviour.SpellEffect.ALTER;
        
        if (multiTag.HasTag(MultiTag.MultiTags.Possessable))
            return AbilityBehaviour.SpellEffect.POSSESS;
        
        if (multiTag.HasTag(MultiTag.MultiTags.Awakenable))
            return AbilityBehaviour.SpellEffect.AWAKEN;
        
        return AbilityBehaviour.SpellEffect.NONE;
    }

    private MultiTag.MultiTags GetMultiTagsByTarget(AbilityBehaviour.SpellTarget value)
    {
        switch (value)
        {
            case AbilityBehaviour.SpellTarget.NATURE:
                return MultiTag.MultiTags.Nature;
            case AbilityBehaviour.SpellTarget.MACHINA:
                return MultiTag.MultiTags.Machina;
            case AbilityBehaviour.SpellTarget.CORRUPTION:
                return MultiTag.MultiTags.Corruption;
        }
        return MultiTag.MultiTags.None;
    }
    private MultiTag.MultiTags GetSpecificMultiTagsByMultiTag(MultiTag multiTag)
    {
        if (multiTag.HasTag(MultiTag.MultiTags.Corruption))
            return MultiTag.MultiTags.Corruption;
        
        if (multiTag.HasTag(MultiTag.MultiTags.Machina))
            return MultiTag.MultiTags.Machina;
        
        if (multiTag.HasTag(MultiTag.MultiTags.Nature))
            return MultiTag.MultiTags.Nature;
        
        return MultiTag.MultiTags.None;
    }

    public List<SpellEffectRunTimeData> GetAllUnlockedSpellEffectRunTimeDatas()
    {
        List<SpellEffectRunTimeData> returnSpellEffectRunTimeDatas = new();
        
        foreach (var spellEffectRunTimeDataKeyVal in spellEffectRunTimeDataByInput)
        {
            SpellEffectRunTimeData spellEffectRunTimeData = spellEffectRunTimeDataKeyVal.Value;
            
            if (spellEffectRunTimeData.IsLocked()) continue;
            
            returnSpellEffectRunTimeDatas.Add(spellEffectRunTimeData);
        }
        
        return returnSpellEffectRunTimeDatas;
    }
    public bool TryGetUnlockedSpellEffectRunTimeData(InputSpellInput input, out SpellEffectRunTimeData spellEffectRunTimeData)
    {
        if(!spellEffectRunTimeDataByInput.TryGetValue(input, out spellEffectRunTimeData)) return false;
        
        if(spellEffectRunTimeData == null) return false;
        
        if(spellEffectRunTimeData.IsLocked()) return false;
        
        return true;
    }
    public bool TryGetUnlockedSpellEffectRunTimeData(AbilityBehaviour.SpellEffect spellEffectEnum, out SpellEffectRunTimeData spellEffectRunTimeData)
    {
        if(!spellEffectRunTimeDataBySpellEffectEnum.TryGetValue(spellEffectEnum, out spellEffectRunTimeData)) return false;
        
        if(spellEffectRunTimeData == null) return false;
        
        if(spellEffectRunTimeData.IsLocked()) return false;
        
        return true;
    }
    
    public bool TryGetUnlockedSpellTarget(MultiTag multiTag, out SO_SpellPart_Target spellTarget)
    {
        spellTarget = null;
        
        MultiTag.MultiTags key = GetSpecificMultiTagsByMultiTag(multiTag);
        if (key == MultiTag.MultiTags.None) return false;
        
        if(!gameUsableSpellTargetsMap.TryGetValue(key, out SO_SpellPart_Target _spellTarget)) return false;
        
        //todo: check if value is locked
        
        spellTarget = _spellTarget;
        return true;
    }
    #endregion
}