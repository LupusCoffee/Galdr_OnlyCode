// Made by Isabelle H. Heiskanen

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JournalPage : MonoBehaviour {
    
    public GameObject tab;

    public void OpenTab() {
        tab.GetComponent<Image>().sprite = JournalManager.Instance.tabIn;
    }

    public void CloseTab() {
        tab.GetComponent<Image>().sprite = JournalManager.Instance.tabOut;
    }
}
