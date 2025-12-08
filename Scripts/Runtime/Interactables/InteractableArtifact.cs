using UnityEngine;

public class InteractableArtifact : InteractableCollectable {

    protected override void OnEnable() { base.OnEnable(); }

    protected override void Start() {
        base.Start();
        //Debug.Log($"ID {GetID()}");
    }

    public override SO_InteractableData Interact() { return this.data; }

    public override bool ChangeState(InteractionState interactionState) {
        if (interactionState == InteractionState.OFF || interactionState == InteractionState.ON) {
            this.state = interactionState;
            return true;
        }
        return false;
    }

    public override string ToString() {
        return ("[InteractableArtifact, ID=" + this.data.GetID() + 
            ", InteractionType=" + this.data.GetInteractionType() +
            ", InteractionState=" + this.state + 
            ", ArtifactName=" + ((SO_InteractableArtifactData)this.data).GetCollectableName() +
            ", LoreText=" + ((SO_InteractableArtifactData)this.data).GetLoreText() + "]");
    }
}
