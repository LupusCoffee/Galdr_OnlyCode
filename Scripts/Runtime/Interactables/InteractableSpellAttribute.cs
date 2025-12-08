using UnityEngine;

public class InteractableSpellAttribute : InteractableCollectable {
    protected override void OnEnable() { base.OnEnable(); }

    protected override void Start() {
        base.Start();
        //Debug.Log($"ID {GetID()}");
    }

    public override SO_InteractableData Interact() { return this.data; }

    public override bool ChangeState(InteractionState interactionState) {
        this.state = interactionState;
        return true;
    }

    public override string ToString() {
        return ("[InteractableSpellAttribute, ID=" + this.data.GetID() + 
            ", InteractionType=" + this.data.GetInteractionType() +
            ", InteractionState=" + this.state + ", SpellAttributeName=" + 
            ((SO_InteractableSpellAttributeData)this.data).GetCollectableName() + "]");
    }
}
