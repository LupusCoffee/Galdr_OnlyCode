using System;
using UnityEditor.Experimental;
using UnityEngine;

public class PlayerEncounterZone : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.TryGetComponent(out WorldEventObject worldEvent)) {
            if (!EncounterManager.Instance.CheckIfWorldEventEncountered(worldEvent.GetID())) {
                Debug.Log("Encounter!");
                EncounterManager.Instance.SubscribeAddEncounter(this.gameObject);
                EncounterManager.Instance.Encounter(worldEvent.GetWorldEvent());
                EncounterManager.Instance.UnsubscribeAddEncounter();
                Debug.Log(EncounterManager.Instance.GetLastWorldEventID());

                bool firstEncounter = Inventory.TryCollectItem(EncounterManager.Instance.GetLastWorldEvent());

                if (firstEncounter) {
                    Debug.Log("This world event is Player's first encounter!");
                    JournalManager.Instance.OpenJournalToPage(1);
                } else {
                    Debug.Log("Not the first encounter.");
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        /*if (other.gameObject.TryGetComponent(out WorldEventObject worldEvent)) {

        }*/
    }
}
