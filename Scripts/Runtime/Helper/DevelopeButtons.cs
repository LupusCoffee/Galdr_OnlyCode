using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class DevelopeButtons : MonoBehaviour
{
    [SerializeField] bool devModeOn;
    [SerializeField] bool unlockAllSpellsAtStart;

    List<SO_SpellAttribute> attributes = new List<SO_SpellAttribute>();

    #region Input Setup
    private void OnEnable()
    {
        UserInputs.Instance._devButton1.performed += UnlockAllSpells;
        UserInputs.Instance._devButton2.performed += LockAllSpells;
    }
    private void OnDisable()
    {
        UserInputs.Instance._devButton1.performed -= UnlockAllSpells;
        UserInputs.Instance._devButton2.performed -= LockAllSpells;
    }
    #endregion


    private void Start()
    {
        foreach (SongAttributeItem item in Inventory.LookupSpellAttributeTable.Values)
            attributes.Add(item.SpellAttribute);

        if (unlockAllSpellsAtStart) UnlockAllSpells();
    }

    private void UnlockAllSpells()
    {
        foreach (var attribute in attributes)
            Inventory.TryCollectItem(attribute);
    }
    private void UnlockAllSpells(InputAction.CallbackContext obj)
    {
        if (!devModeOn) return;

        foreach (var attribute in attributes)
            Inventory.TryCollectItem(attribute);
    }
    private void LockAllSpells(InputAction.CallbackContext obj)
    {
        if (!devModeOn) return;
    }
}
