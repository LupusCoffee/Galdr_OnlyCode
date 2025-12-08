using UnityEngine;

[CreateAssetMenu(fileName = "SO_InteractableSpellAttributeData", 
    menuName = "Scriptable Objects/Interactables/SpellAttribute Data")]
public class SO_InteractableSpellAttributeData : SO_InteractableCollectableData {
    [SerializeField] private SO_SpellAttribute so = null;

    protected override void OnEnable() {
        base.OnEnable();
        this.type = InteractionType.SPELL_ATTRIBUTE;
        if (this.collectableName == "default_collectable_name") { 
            this.collectableName = "default_spellattribute_name";
        }
    }

    public SO_SpellAttribute GetSpellAttribute() { return this.so; }

    public bool SetSpellAttribute(SO_SpellAttribute so) {
        this.so = so;
        return true;
    }

    public override bool ChangeData(int interactableID, InteractionType type) {
        if (interactableID < 0) { return false; }
        this.interactableID = interactableID;
        this.type = InteractionType.SPELL_ATTRIBUTE;
        return true;
    }

    public bool ChangeData(string spellAttributeName, SO_SpellAttribute so) {
        this.collectableName = spellAttributeName;
        this.so = so;
        return true;
    }

    public bool ChangeData(int interactableID, string spellAttributeName) {
        if (interactableID < 0) { return false; }
        this.interactableID = interactableID;
        this.collectableName = spellAttributeName;
        return true;
    }

    public bool ChangeData(int interactableID, string spellAttributeName, SO_SpellAttribute so) {
        if (interactableID < 0) { return false; }
        this.interactableID = interactableID;
        this.collectableName = spellAttributeName;
        this.so = so;
        return true;
    }
}
