using UnityEngine;

public class WorldEventObject : MonoBehaviour {
    [SerializeField] private SO_WorldEvents worldEvent;
    
    public SO_WorldEvents GetWorldEvent() { return this.worldEvent; }

    public int GetID() { return this.worldEvent.ID; }
}
