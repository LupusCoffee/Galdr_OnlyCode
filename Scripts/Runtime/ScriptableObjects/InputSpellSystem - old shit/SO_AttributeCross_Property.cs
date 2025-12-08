using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "SO_AttributeCross_Property", menuName = "Scriptable Objects/SO_AttributeCross_Property")]
public class SO_AttributeCross_Property : ScriptableObject
{
    [SerializeField] private List<SO_SpellAtt_Property> spellAttributes;
    public List<SO_SpellAtt_Property> GetAttributes() => spellAttributes;
    public SO_SpellAtt_Property GetAttributeAt(int i)
    {
        if (i >= spellAttributes.Count) return null;
        else return spellAttributes[i];
    }
}
