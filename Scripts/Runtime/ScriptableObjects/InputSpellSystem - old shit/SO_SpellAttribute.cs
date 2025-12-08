using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SO_SpellAttribute : ScriptableObject
{
    [SerializeField] private string attributeName;
    [SerializeField] private Sprite attributeUnlockedIconSprite;
    [SerializeField] private Sprite attributeLockedIconSprite;
    [SerializeField] private Sprite attributeWaveSprite;
    [SerializeField] private GameObject spellParticles;
    [SerializeField] private AK.Wwise.Event musicNote;
    [SerializeField] private AK.Wwise.Event musicFinish;
    [SerializeField] private List<InputSpellInput> typeNoteSequence;
    public string GetName() => attributeName;
    public Sprite GetUnlockedIconSprite() => attributeUnlockedIconSprite;
    public Sprite GetLockedIconSprite() => attributeLockedIconSprite;
    public Sprite GetWaveSprite() => attributeWaveSprite;
    public GameObject GetSpellParticles() => spellParticles;
    public AK.Wwise.Event GetMusicNote() => musicNote;
    public AK.Wwise.Event GetMusicFinish() => musicFinish;
    public List<InputSpellInput> GetNoteSequence() => typeNoteSequence;
}
