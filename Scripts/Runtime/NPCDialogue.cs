using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogue : Interactable
{
    [Space(20)]
    [Header("Setup")]
    [SerializeField] GameObject dialogueCanvas;
    [SerializeField] GameObject optionsParent;
    [SerializeField] GameObject dialogueOptionPrefab;

    [SerializeField] GameObject answerContainer;
    [SerializeField] TextMeshProUGUI answerText;


    [Space(20)]
    [Header("Dialogue")]
    [SerializeField] private Dialogue[] dialogueOptions;
    private int activeDialogue;
    private bool understandCurrentDialogueIndex; //otherwise encrypt the messages
    private int dialogueProgressIndex = 0;


    public override SO_InteractableData Interact()
    {
        DisplayOptions();

        //fluteplayer disable UI??!!?!?!?!??!?!?
        Player.Instance.DisableControls(null);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        return data;
    }

    private void DisplayOptions()
    {
        dialogueCanvas.SetActive(true);
        optionsParent.SetActive(true);
        answerContainer.SetActive(false);

        //clear the options parent
        foreach (Transform child in optionsParent.transform) {
            Destroy(child.gameObject);
        }

        //foreach dialogueoptions, instantiate a textmeshpro object with the dialogue.question

        for (int i = 0; i < dialogueOptions.Length; i++)
        {
            GameObject dialogueOption = Instantiate(dialogueOptionPrefab, optionsParent.transform);
            TextMeshProUGUI text = dialogueOption.GetComponentInChildren<TextMeshProUGUI>();
            text.text = dialogueOptions[i].question;

            //if important question, set text color to yellow
            if (dialogueOptions[i].importantQuestion) {
                text.color = Color.yellow;
            }

            int index = i;
            dialogueOption.GetComponent<Button>().onClick.AddListener(() => ActivateDialogue(index));
        }
    }

    private void ActivateDialogue(int index)
    {
        activeDialogue = index;

        optionsParent.SetActive(false);
        answerContainer.SetActive(true);

        
        //check if the player has the required language level to understand the dialogue using Player.Instance.LanguageLevel
        understandCurrentDialogueIndex = Player.Instance.LanguageLevel >= dialogueOptions[activeDialogue].langReq;
        Debug.Log("Understand current dialogue: " + understandCurrentDialogueIndex + " because player dialogue level is " + Player.Instance.LanguageLevel);

        dialogueProgressIndex = 0;
        DisplayCurrentDialogue();
    }

    public void ProgressStory()
    {
        if (dialogueProgressIndex < dialogueOptions[activeDialogue].answers.Length)
        {
            DisplayCurrentDialogue();
        }
        else
        {
            Player.Instance.EnableControls();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            dialogueCanvas.SetActive(false);
            optionsParent.SetActive(false);
            dialogueProgressIndex = 0;
        }
    }

    private void DisplayCurrentDialogue()
    {
        // display the current dialogue if it meets language level requirements, otherwise encrypt it
        string dialogue = LanguageEncrypter.TryGetText(dialogueOptions[activeDialogue].answers[dialogueProgressIndex], dialogueOptions[activeDialogue].langReq);

        answerText.text = dialogue;
        dialogueProgressIndex++;
    }
}

[System.Serializable]
public class Dialogue
{
    public string question;
    public string[] answers;
    public int langReq;
    public bool importantQuestion;
}
