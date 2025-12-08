using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "SO_AttributeCross", menuName = "Scriptable Objects/SO_AttributeCross")]
public class SO_AttributeCross : ScriptableObject
{
    [SerializeField] private List<SO_SpellAttribute> spellAttributes;
    public List<SO_SpellAttribute> GetAttributes() => spellAttributes;
}
