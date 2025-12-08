using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class InputSpellNote
{
    [FormerlySerializedAs("posOnNoteSheet")] [SerializeField] [UnityEngine.Range(0, 10)]
    private int posOnBar;
    
    [SerializeField]
    private InputSpellInput inputButton;
    
    [SerializeField]
    private AK.Wwise.Event sound;
    
    //Getters
    public int GetPosOnBar() => posOnBar;
    public InputSpellInput GetInputButton() => inputButton;
    public AK.Wwise.Event GetSound() => sound;
}
