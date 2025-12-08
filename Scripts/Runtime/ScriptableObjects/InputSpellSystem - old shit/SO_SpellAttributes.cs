using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

//[CreateAssetMenu(fileName = "SO_SpellAttributes", menuName = "Scriptable Objects/SO_SpellAttributes")]
public class SO_SpellAttributes : ScriptableObject
{
    //AAAAAAAAAAAAAAAAGGGGGGGGGGGHHHHHHHHHHH REPEATED CODE NOOOOO AHHHHHH MY PTSD IS COMING BAAAAAAACK AAAAAAAAHHHHHHH - Mohammed

    [Serializable]
    public class SpellAttribute
    {
        [SerializeField] private List<InputSpellInput> typeNoteSequence;
        public List<InputSpellInput> GetNoteSequence() => typeNoteSequence;
    }
    
    [Serializable]
    public class TypeAttribute : SpellAttribute
    {
        [SerializeField] private MusicManager.MusicScales musicScaleOnActive;
        [FormerlySerializedAs("spellType")] [SerializeField] private AbilityBehaviour.SpellEffect spellEffect;
        public MusicManager.MusicScales GetScaleToActivate() => musicScaleOnActive;
        public AbilityBehaviour.SpellEffect GetSpellType() => spellEffect;
    }
    
    [Serializable]
    public class PropertyAttribute : SpellAttribute
    {
        [SerializeField] private MusicManager.MusicScales musicScaleOnActive;
        //[SerializeField] AbilityBehaviour.SpellProperties spellProperty;
        public MusicManager.MusicScales GetScaleToActivate() => musicScaleOnActive;
        //public AbilityBehaviour.SpellProperties GetSpellProperty() => spellProperty;
    }
    
    [Serializable]
    public class TargetAttribute : SpellAttribute
    {
        [SerializeField] private AbilityBehaviour.SpellTarget spellTarget;
        public AbilityBehaviour.SpellTarget GetSpellTarget() => spellTarget;
    }

    [SerializeField] private int typeAttributeLength;
    [SerializeField] private List<TypeAttribute> typeAttributes;

    [SerializeField] private int propertyAttributeLength;
    [SerializeField] private List<PropertyAttribute> propertyAttributes;

    [SerializeField] private int targetAttributeLength;
    [SerializeField] private List<TargetAttribute> targetAttributes;


    public int GetTypeAttLength() => typeAttributeLength;
    public int GetPropertyAttLength() => propertyAttributeLength;
    public int GetTargetAttLength() => targetAttributeLength;

    public List<TypeAttribute> GetTypeAttributes() => typeAttributes;
    public List<PropertyAttribute> GetPropertyAttribute() => propertyAttributes;
    public List<TargetAttribute> GetTargetAttribute() => targetAttributes;
}
