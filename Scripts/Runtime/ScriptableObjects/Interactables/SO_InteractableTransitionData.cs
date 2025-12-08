using UnityEngine;

[CreateAssetMenu(fileName = "SO_InteractableTransitionData", 
    menuName = "Scriptable Objects/Interactables/Transition Data")]
public class SO_InteractableTransitionData : SO_InteractableData {
    [SerializeField] private string transitionName;

    protected override void OnEnable() {
        base.OnEnable();
        this.type = InteractionType.TRANSITION_TO_AREA;
        if (string.IsNullOrEmpty(this.transitionName)) { this.transitionName = "default_door_name"; }
    }

    public override void SetInteractionType(InteractionType type) { 
        this.type = InteractionType.TRANSITION_TO_AREA; 
    }

    public string GetTransitionName() { return this.transitionName; }

    public void SetTransitionName(string transitionName) { this.transitionName = transitionName; }

    public override bool ChangeData(int interactableID, InteractionType type) {
        if (interactableID < 0) { return false; }
        this.interactableID = interactableID;
        this.type = InteractionType.TRANSITION_TO_AREA;
        return true;
    }

    public bool ChangeData(int interactableID, string transitionName) {
        if (interactableID < 0) { return false; }
        this.interactableID = interactableID;
        this.transitionName = transitionName;
        return true;
    }
}
