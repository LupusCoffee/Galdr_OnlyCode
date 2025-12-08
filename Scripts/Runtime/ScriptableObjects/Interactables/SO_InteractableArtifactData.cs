using UnityEngine;

[CreateAssetMenu(fileName = "SO_InteractableArtifactData", 
    menuName = "Scriptable Objects/Interactables/Artifact Data")]
public class SO_InteractableArtifactData : SO_InteractableCollectableData {
    [SerializeField] private string loreText;

    protected override void OnEnable() {
        base.OnEnable();
        if (this.collectableName == "default_collectable_name") {
            this.collectableName = "default_artifact_name";
        }
        if (string.IsNullOrEmpty(this.loreText)) { this.loreText = "default_lore_text"; }
    }

    public string GetLoreText() { return this.loreText; }

    public void SetLoreText(string loreText) { this.loreText = loreText; }

    public override bool ChangeData(int interactableID, InteractionType type) {
        if (interactableID < 0) { return false; }
        this.interactableID = interactableID;
        this.type = InteractionType.ITEM;
        return true;
    }

    public bool ChangeData(int interactableID, string loreText) {
        if (interactableID < 0) { return false; }
        this.interactableID = interactableID;
        this.loreText = loreText;
        return true;
    }

    public bool ChangeData(string artifactName, string loreText) {
        this.collectableName = artifactName;
        this.loreText = loreText;
        return true;
    }

    public bool ChangeData(int interactableID, string artifactName, string loreText) {
        this.interactableID = interactableID;
        this.collectableName = artifactName;
        this.loreText = loreText;
        return true;
    }
}
