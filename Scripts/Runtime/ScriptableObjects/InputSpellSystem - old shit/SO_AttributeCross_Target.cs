using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "SO_AttributeCross_Target", menuName = "Scriptable Objects/SO_AttributeCross_Target")]
public class SO_AttributeCross_Target : ScriptableObject
{
    [SerializeField] private List<SO_SpellAtt_Target> spellAttributes;
    public List<SO_SpellAtt_Target> GetAttributes() => spellAttributes;
    public SO_SpellAtt_Target GetAttributeAt(int i)
    {
        if (i >= spellAttributes.Count) return null;
        else return spellAttributes[i];
    }
}
