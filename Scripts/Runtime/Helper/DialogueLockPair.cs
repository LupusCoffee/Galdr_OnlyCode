using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class DialogueLockPair
{
    public int languageLevel;
    public string dialogue;

    public (bool encrypted, string text) GetDialogue()
    {
        if(FirstPersonController.Instance != null)
        {
            if (FirstPersonController.Instance.LanguageLevel >= languageLevel)
            {
                return (false, dialogue);
            }
            else
            {
                return (true, LanguageEncrypter.EncryptText(dialogue));
            }
        }


        if (Player.Instance.LanguageLevel >= languageLevel)
        {
            return (false, dialogue);
        }
        else
        {
            return (true, LanguageEncrypter.EncryptText(dialogue));
        }
    }
}

