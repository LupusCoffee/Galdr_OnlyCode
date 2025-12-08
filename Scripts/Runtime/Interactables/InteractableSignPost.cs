using UnityEngine;

public class InteractableSignPost : Interactable {

    protected override void OnEnable() { base.OnEnable(); }

    protected override void Start() { base.Start(); }

    void Update() {}

    public override SO_InteractableData Interact() { return this.data; }

    public override bool ChangeState(InteractionState interactionState) {
        this.state = interactionState;
        return true; 
    }

    public override string ToString() {
        return ("[InteractableSignPost, ID=" + this.data.GetID() + 
            ", InteractionType=" + this.data.GetInteractionType() +
            ", InteractionState=" + this.state + 
            ", SignText=" + ((SO_InteractableSignPostData)this.data).GetSignText() + "]");
    }
}
