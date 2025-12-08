using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "SO_AttributeCross_Type", menuName = "Scriptable Objects/SO_AttributeCross_Type")]
public class SO_AttributeCross_Type : ScriptableObject
{
    [SerializeField] private List<SO_SpellAtt_Type> spellAttributes;

    public List<SO_SpellAtt_Type> GetAttributes() => spellAttributes;
    public SO_SpellAtt_Type GetAttributeAt(int i)
    {
        if (i >= spellAttributes.Count) return null;
        else return spellAttributes[i];
    }
}
