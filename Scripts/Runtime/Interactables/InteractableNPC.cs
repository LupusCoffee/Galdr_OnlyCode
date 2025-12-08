using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNPC : Interactable {
    protected override void OnEnable() { base.OnEnable(); }

    protected override void Start() { base.Start(); }

    public override SO_InteractableData Interact() {
        /*GameObject gameObject = Instantiate(((InteractableNPCData)this.data.GetInteractableData()).GetDialogueBoxPopup(), 
            transform.position + new Vector3(0, 2.5F, 0), Quaternion.identity);
        gameObject.transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one, 0.25f);
        StartCoroutine(TextDecay(gameObject));*/

        return this.data;
    }

    private IEnumerator TextDecay(GameObject target) {
        yield return new WaitForSeconds(3);
        LeanTween.scale(target, Vector3.zero, 0.1f).setOnComplete(()
            => Destroy(target));
    }

    public override bool ChangeState(InteractionState interactionState) {
        this.state = interactionState;
        return true;
    }

    public override string ToString() {
        return ("[InteractableNPC, ID=" + this.data.GetID() + 
            ", InteractionType=" + this.data.GetInteractionType() +
            ", InteractionState=" + this.state + 
            ", NPCName=" + ((SO_InteractableNPCData)this.data).GetNPCName() + "]");
    }
}
