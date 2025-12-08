using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class LanguageEncrypter
{
    public static TMP_SpriteAsset unknownSymbols;
    private static List<string> spriteNames = new List<string>();

    public static void SetUnknownSymbols(TMP_SpriteAsset asset)
    {
        unknownSymbols = asset;
    }

    private static void InitializeSpriteNames()
    {
        if (spriteNames.Count == 0)
        {
            foreach (TMP_SpriteCharacter sprite in unknownSymbols.spriteCharacterTable)
            {
                spriteNames.Add(sprite.name);
            }
        }
    }

    public static string TryGetText(string text, int languageLevel)
    {
        Debug.Log("Player language level is " + Player.Instance.LanguageLevel + " and language level required is " + languageLevel);

        if (Player.Instance.LanguageLevel >= languageLevel)
        {
            return text;
        }
        else
        {
            Debug.Log("Encrypting text");
            return EncryptText(text);
        }
    }

    public static string EncryptText(string text)
    {
        if (unknownSymbols == null)
        {
            unknownSymbols = Resources.Load<TMP_SpriteAsset>("Fonts/EncryptedFont");
        }

        if (spriteNames.Count == 0)
        {
            InitializeSpriteNames();
        }
        string encryptedText = "";

        int count = (text.Split(' ').Length - 1) / 2;

        if (count < 5) count = 5;

        for (int i = 0; i < count; i++)
        {
            string temp = spriteNames[Random.Range(0, spriteNames.Count)];
            encryptedText += $"<sprite name=\"{temp}\" tint=1/>";
        }

        return encryptedText;
    }
}