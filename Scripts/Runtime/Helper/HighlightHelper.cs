using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HighlightHelper : MonoBehaviour
{
    private List<SpellCastableObject> spellCastableObjectsToHighlight = new();
    
    public void UpdateHighlightsOnMultipleObjects(List<SpellCastableObject> spellCastableObjectsInProximity)
    {
        foreach (var currentFrameSpellCastableObject in spellCastableObjectsInProximity)
        {
            if (spellCastableObjectsToHighlight.Contains(currentFrameSpellCastableObject)) continue;
                
            if(!currentFrameSpellCastableObject.TryGetMultiTag(out var multiTag)) continue;
            if(!currentFrameSpellCastableObject.TryGetOutlineObject(out var outlineObject)) continue;
            
            EnableHighlight(multiTag, outlineObject);
            spellCastableObjectsToHighlight.Add(currentFrameSpellCastableObject);
        }

        for (int i = spellCastableObjectsToHighlight.Count - 1; i >= 0; i--)
        {
            SpellCastableObject lastFrameSpellCastableObject = spellCastableObjectsToHighlight[i];
            if(spellCastableObjectsInProximity.Contains(lastFrameSpellCastableObject)) continue;
            
            if(!lastFrameSpellCastableObject.TryGetOutlineObject(out var outlineObject)) continue;
            outlineObject.DisableAllHighlights();
            spellCastableObjectsToHighlight.Remove(lastFrameSpellCastableObject);
        }
    }
    public void UpdateHighlightOnSingleObject(SpellCastableObject spellCastableObjectInProximity)
    {
        if(!spellCastableObjectInProximity.TryGetMultiTag(out var multiTag)) return;
        if(!spellCastableObjectInProximity.TryGetOutlineObject(out var outlineObject)) return;
        
        DisableHighlightOnAllObjects(); //todo: come up with a way to not disable every tick *sigh*
        EnableHighlight(multiTag, outlineObject);
    }
    
    public void ReapplyHighlights()
    {
        foreach (var spellCastableObject in spellCastableObjectsToHighlight)
        {
            if(!spellCastableObject.TryGetMultiTag(out var multiTag)) continue;
            if(!spellCastableObject.TryGetOutlineObject(out var outlineObject)) continue;
            EnableHighlight(multiTag, outlineObject);
        }
    }
    public void DisableHighlightOnAllObjects()
    {
        foreach (var spellCastableObject in spellCastableObjectsToHighlight)
        {
            if(!spellCastableObject.TryGetOutlineObject(out var outlineObject)) continue;
            outlineObject.DisableAllHighlights();
        }
    }
    
    private void EnableHighlight(MultiTag objectMultiTag, OutlineObject outlineObject)
    {
        AbilityBehaviour.SpellEffect objectSpellEffectEnum = 
            SpellManager.Instance.GetSpellEffectEnumByMultiTag(objectMultiTag);
        
        switch (objectSpellEffectEnum)
        {
            case AbilityBehaviour.SpellEffect.ALTER:
                outlineObject.SetOutlineGreen(true); //do these better
                break;
            case AbilityBehaviour.SpellEffect.POSSESS:
                outlineObject.SetOutlinePink(true);
                break;
            case AbilityBehaviour.SpellEffect.AWAKEN:
                outlineObject.SetOutlineBlue(true);
                break;
        }
    }
}
