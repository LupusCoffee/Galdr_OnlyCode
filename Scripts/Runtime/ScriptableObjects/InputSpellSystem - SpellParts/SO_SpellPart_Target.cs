using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellPart_Target_", menuName = "Scriptable Objects/Spells/SO_SpellPart_Target")]
public class SO_SpellPart_Target : ScriptableObject
{
    [SerializeField] 
    private AbilityBehaviour.SpellTarget targetEnum;
    
    [SerializeField] 
    private List<InputSpellNote> noteSequence = new();
    
    //Getters
    public AbilityBehaviour.SpellTarget GetSpellTargetEnum() => targetEnum;
    public List<InputSpellNote> GetNoteSequence() => noteSequence;
}
