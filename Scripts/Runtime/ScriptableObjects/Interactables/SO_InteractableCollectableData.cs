using UnityEngine;

public abstract class SO_InteractableCollectableData : SO_InteractableData {
    [SerializeField] protected string collectableName;

    protected override void OnEnable() {
        base.OnEnable();
        this.type = InteractionType.ITEM;
        if (string.IsNullOrEmpty(this.collectableName)) { this.collectableName = "default_collectable_name"; }
    }

    public override void SetInteractionType(InteractionType type) { this.type = InteractionType.ITEM; }

    public string GetCollectableName() { return this.collectableName; }

    public void SetCollectableName(string collectableName) { this.collectableName = collectableName; }
}
