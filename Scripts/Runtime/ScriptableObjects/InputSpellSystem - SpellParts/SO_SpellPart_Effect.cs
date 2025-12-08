using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SpellPart_Effect_", menuName = "Scriptable Objects/Spells/SO_SpellPart_Effect")]
public class SO_SpellPart_Effect : ScriptableObject
{
    [SerializeField] 
    private Sprite availableSprite;
    
    [SerializeField] 
    private Sprite unavailableSprite;
    
    [SerializeField] 
    private AbilityBehaviour.SpellEffect effectEnum;
    
    [SerializeField] 
    private AK.Wwise.Event soundUponChosen;
    
    
    public Sprite GetSpellEffectIcon(bool isAvailable)
    {
        return isAvailable ? availableSprite : unavailableSprite;
    }
    public AbilityBehaviour.SpellEffect GetSpellEffectEnum() => effectEnum;
    public AK.Wwise.Event GetSpellEffectSound() => soundUponChosen;
}
