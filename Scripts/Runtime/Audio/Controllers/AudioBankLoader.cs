//Created by Mohammed (the sex god)

using AK.Wwise;
using System.Collections.Generic;
using UnityEngine;

public class AudioBankLoader : MonoBehaviour
{
    [SerializeField] private List<AK.Wwise.Bank> banksToLoad;
    private List<AK.Wwise.Bank> loadedBanks = new List<Bank>();


    public void LoadBank(AK.Wwise.Bank bank)
    {
        bank.Load();
        loadedBanks.Add(bank);
    }
    public void UnloadBank(AK.Wwise.Bank bank)
    {
        for (int i = 0; i < loadedBanks.Count; i++)
        {
            if (bank == loadedBanks[i])
            {
                bank.Unload();
                loadedBanks.RemoveAt(i);
            }
        }
    }
    public void LoadAllBanks()
    {
        foreach (var bank in banksToLoad) LoadBank(bank);
    }
    public void UnloadAllBanks()
    {
        for (int i = 0; i < loadedBanks.Count; i++)
        {
            loadedBanks[i].Unload();
            loadedBanks.RemoveAt(i);
        }
    }

    private void OnEnable()
    {
        LoadAllBanks();
    }
    private void OnDisable()
    {
        UnloadAllBanks();
    }
}
