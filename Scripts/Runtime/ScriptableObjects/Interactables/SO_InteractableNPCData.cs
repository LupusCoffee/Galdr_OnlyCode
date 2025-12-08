using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_InteractableNPCData", 
    menuName = "Scriptable Objects/Interactables/NPC Data")]
public class SO_InteractableNPCData : SO_InteractableData {
    [SerializeField] private string npcName;
    [SerializeField] private List<string> dialogues = new List<string>();
    [SerializeField] private GameObject dialogueBoxPopup = null;

    protected override void OnEnable() {
        base.OnEnable();
        this.type = InteractionType.VERBAL;
        if (string.IsNullOrEmpty(this.npcName)) { this.npcName = "Larry"; }
        if (this.dialogues.Count == 0) { this.dialogues.Add("empty_dialogue"); }
    }

    public override void SetInteractionType(InteractionType type) { this.type = InteractionType.VERBAL; }

    public string GetNPCName() { return this.npcName; }

    public void SetNPCName(string npcName) { this.npcName = npcName; }

    public string GetFirstDialogue() { return this.dialogues[0]; }

    public GameObject GetDialogueBoxPopup() { return this.dialogueBoxPopup; }

    public void SetDialogueBoxPopup(GameObject dialogueBoxPopup) { this.dialogueBoxPopup = dialogueBoxPopup; }

    public string GetDialogue(int index) {
        if (index < 0 || index >= this.dialogues.Count)
            return "Index out of bounds.";
        return this.dialogues[index];
    }

    public List<string> GetDialogues() { return this.dialogues; }

    public void ChangeAllDialogue(List<string> dialogues) {
        this.dialogues.Clear();
        if (dialogues.Count == 0) {
            this.dialogues.Add("empty_dialogue");
            return;
        }
        this.dialogues = dialogues;
    }

    public void AddDialogue(string dialogue) {
        //maybe needs a separate function to get rid of all the "empty_dialogue" elements?
        if (this.dialogues.Count == 1 && this.dialogues[0] == "empty_dialogue") {
            this.dialogues.RemoveAt(0);
        }
        this.dialogues.Add(dialogue); 
    }

    public bool RemoveDialogue(int index) {
        if (index < 0 || index >= this.dialogues.Count) { return false; }
        this.dialogues.RemoveAt(index);
        if (this.dialogues.Count == 0) { this.dialogues.Add("empty_dialogue"); }
        return true;
    }

    public void RemoveAllDialogue() {
        this.dialogues.Clear();
        this.dialogues.Add("empty_dialogue");
    }

    public override bool ChangeData(int interactableID, InteractionType type) {
        if (interactableID < 0) { return false; }
        this.interactableID = interactableID;
        this.type = InteractionType.VERBAL;
        return true;
    }

    public bool ChangeData(int interactableID, string npcName) {
        if (interactableID < 0) { return false; }
        this.interactableID = interactableID;
        this.npcName = npcName;
        return true;
    }

    public void ChangeData(string npcName, List<string> dialogues) {
        this.npcName = npcName;
        this.ChangeAllDialogue(dialogues);
    }

    public bool ChangeData(int interactableID, string npcName, List<string> dialogues) {
        if (interactableID < 0) { return false; }
        this.interactableID = interactableID;
        this.npcName = npcName;
        this.ChangeAllDialogue(dialogues);
        return true;
    }
}
