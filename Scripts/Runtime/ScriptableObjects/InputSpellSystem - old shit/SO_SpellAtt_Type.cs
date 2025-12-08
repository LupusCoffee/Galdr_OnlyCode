using UnityEngine;
using UnityEngine.Serialization;

//[CreateAssetMenu(fileName = "SO_Type_", menuName = "Scriptable Objects/SpellAttribute/SO_SpellAtt_Type")]
public class SO_SpellAtt_Type : SO_SpellAttribute
{
    [FormerlySerializedAs("spellType")] [SerializeField] private AbilityBehaviour.SpellEffect spellEffect;
    public AbilityBehaviour.SpellEffect GetSpellType() => spellEffect;
}
