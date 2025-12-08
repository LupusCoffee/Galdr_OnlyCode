using UnityEngine;

[CreateAssetMenu(fileName = "SO_InteractableDoorData", 
    menuName = "Scriptable Objects/Interactables/Door Data")]
public class SO_InteractableDoorData : SO_InteractableData {
    [SerializeField] private string doorName;

    protected override void OnEnable() {
        base.OnEnable();
        this.type = InteractionType.TRANSITION;
        if (string.IsNullOrEmpty(this.doorName)) { this.doorName = "default_door_name"; }
    }

    public override void SetInteractionType(InteractionType type) { this.type = InteractionType.TRANSITION; }

    public string GetDoorName() { return this.doorName; }

    public void SetDoorName(string doorName) { this.doorName = doorName; }

    public override bool ChangeData(int interactableID, InteractionType type) {
        if (interactableID < 0) { return false; }
        this.interactableID = interactableID;
        this.type = InteractionType.TRANSITION;
        return true;
    }

    public bool ChangeData(int interactableID, string doorName) {
        if (interactableID < 0) { return false; }
        this.interactableID = interactableID;
        this.doorName = doorName;
        return true;
    }
}
