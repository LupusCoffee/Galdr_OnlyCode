using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle_ToggleObjectActiveListener : MonoBehaviour
{
    [SerializeField] PuzzleEvent puzzleEvent;

    [Header("Affected Objects")]
    [SerializeField] List<GameObject> objectsToToggleOn;
    [SerializeField] List<GameObject> objectsToToggleOff;

    [Space(10)]
    [SerializeField] List<SpellActivatable> alterablesToEnable;
    [SerializeField] List<SpellActivatable> alterablesToDisable;
    
    // Added by Martin M
    [Space(10)]
    [Header("Someone remade this script and forgot piece together the legacy code, so here it is for now")]
    [SerializeField] List<Alterable> alterablesToEnableLegacy;
    [SerializeField] List<Alterable> alterablesToDisableLegacy;

    private void Start()
    {
        puzzleEvent.OnButtonPressed += ToggleObjects;
    }

    private void ToggleObjects(GameObject obj)
    {
        foreach (GameObject bojy in objectsToToggleOn)
        {
            bojy.SetActive(true);
        }

        foreach (GameObject bojy in objectsToToggleOff)
        {
            bojy.SetActive(false);
        }

        foreach (SpellActivatable bojy in alterablesToEnable)
        {
            bojy.enabled = true;
        }

        foreach (SpellActivatable bojy in alterablesToDisable)
        {
            bojy.enabled = false;
        }
        
        foreach (Alterable bojy in alterablesToEnableLegacy)
        {
            bojy.enabled = true;
        }
        
        foreach (Alterable bojy in alterablesToDisableLegacy)
        {
            bojy.enabled = false;
        }
    }
}
