using System;
using System.Collections.Generic;
using UnityEngine;

public class UIInputSpellBar : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> uiNotes = new();
    
    [SerializeField]
    private List<UIInput> uiInputs = new();

    public void SetNoteOnBar(InputSpellNote note)
    {
        DisableAllNotes();

        int notePlacement = note.GetPosOnBar();
        uiInputs[notePlacement].SetImageByInput(note.GetInputButton());
        uiNotes[notePlacement].SetActive(true);
    }

    public void DisableAllNotes()
    {
        foreach (var uiNote in uiNotes)
            uiNote.SetActive(false);
    }
}
