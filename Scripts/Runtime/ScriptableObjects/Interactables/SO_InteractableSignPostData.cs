using UnityEngine;

[CreateAssetMenu(fileName = "SO_InteractableSignPostData", 
menuName = "Scriptable Objects/Interactables/Sign Post Data")]
public class SO_InteractableSignPostData : SO_InteractableData {
    [SerializeField] private string signText;

    protected override void OnEnable() {
        base.OnEnable();
        this.type = InteractionType.ORTHOGRAPHIC;
        if (string.IsNullOrEmpty(this.signText)) { this.signText = "Hi"; }
    }

    public override void SetInteractionType(InteractionType type) { this.type = InteractionType.ORTHOGRAPHIC; }

    public string GetSignText() { return this.signText; }

    public void SetSignText(string signText) { this.signText = signText; }

    public override bool ChangeData(int interactableID, InteractionType type) {
        if (interactableID < 0) { return false; }
        this.interactableID = interactableID;
        this.type = InteractionType.ORTHOGRAPHIC;
        return true;
    }

    public bool ChangeData(int interactableID, string signText) {
        if (interactableID < 0) { return false; }
        this.interactableID = interactableID;
        this.signText = signText;
        return true;
    }
}
