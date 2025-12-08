using UnityEngine;

//[CreateAssetMenu(fileName = "SO_Target_", menuName = "Scriptable Objects/SpellAttribute/SO_SpellAtt_Target")]
public class SO_SpellAtt_Target : SO_SpellAttribute
{
    [SerializeField] private AbilityBehaviour.SpellTarget spellTarget;
    public AbilityBehaviour.SpellTarget GetSpellTarget() => spellTarget;
}
