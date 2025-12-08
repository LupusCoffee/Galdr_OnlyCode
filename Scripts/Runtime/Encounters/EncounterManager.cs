using System;
using System.Collections.Generic;
using UnityEngine;

//public delegate void FirstEncounter(EncounterType encounter, int encounterID);
public delegate void FirstEncounter(SO_WorldEvents worldEvent);

/*public enum EncounterType : byte {
    MENTOR = 0,
    VINES = 1,
    FOG = 2,
    DEVOURER = 3,
    THORNSHELL = 4,
    TEST = 5
}*/

public class EncounterManager : MonoBehaviour {
    private static EncounterManager _instance;
    public static EncounterManager Instance {
        get {
            if (_instance == null) {
                _instance = new GameObject("FirstEncounterManager").AddComponent<EncounterManager>();
            }
            return _instance;
        }
    }

    private event FirstEncounter Encountered;
    private bool canSubscribe = true;

    private GameObject currentOwner;

    /**
     * Needs to check if the max amount of available data has been collected to disable to EncounterZone!!
     * Otherwise it could potentially add more worldEvents.
     */
    private readonly List<SO_WorldEvents> worldEvents = new List<SO_WorldEvents>();

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (_instance != null) {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        worldEvents.Clear();
    }

    //public static void Subscribe(GameObject owner, FirstEncounter handler) {
    public void SubscribeAddEncounter(GameObject owner) {
        if (canSubscribe) {
            if (owner != null) {
                //Encountered = null;
                //Encountered += AddEncounter;
                Encountered = AddEncounter;
                canSubscribe = false;
            }

            /*else if (owner == null && handler.Method.Name == "AddEncounter") {
                return;
            } else {
                Encountered += handler;
                if (Encountered.GetInvocationList().Length == 2) {
                    canSubscribe = false;
                }
            }*/
        }
    }

    public void UnsubscribeAddEncounter() {
        //if (Encountered != null && Encountered.GetInvocationList().Length > 0)
        if (Encountered != null && Encountered.GetInvocationList()[0].Method.Name == "AddEncounter")
            Encountered = null;
        canSubscribe = true;
    }

    public void Encounter(SO_WorldEvents worldEvent) {
        Encountered?.Invoke(worldEvent);
    }

    public void AddEncounter(SO_WorldEvents worldEvent) {
        for(int i = 0; i < worldEvents.Count; i++) {
            if (worldEvent.ID == worldEvents[i].ID) { return; }
        }
        worldEvents.Add(worldEvent);
    }

    public bool CheckIfWorldEventEncountered(int id) {
        foreach (SO_WorldEvents worldEvent in worldEvents) {
            if (id == worldEvent.ID) { return true; }
        }
        return false;
    }

    public SO_WorldEvents GetFirstWorldEvent() {  return worldEvents[0]; }

    public SO_WorldEvents GetLastWorldEvent() { return worldEvents[worldEvents.Count-1]; }

    public SO_WorldEvents GetWorldEvent(int index) {
        if (index < 0 || index > worldEvents.Count-1)
            return null;
        return worldEvents[index];
    }

    public int GetFirstWorldEventID() {
        if (worldEvents.Count == 0)
            return -1;
        return worldEvents[0].ID;
    }

    public int GetLastWorldEventID() {
        if (worldEvents.Count == 0)
            return -1;
        return worldEvents[worldEvents.Count-1].ID;
    }

    public int GetWorldEventID(int index) {
        if (index < 0 || index > worldEvents.Count-1)
            return -1;
        return worldEvents[index].ID;
    }

    public int[] GetAllWorldEventIDs() {
        if (worldEvents.Count == 0)
            return new int[] { -1 };

        int[] result = new int[worldEvents.Count];
        for (int i = 0; i < result.Length; i++) {
            result[i] = worldEvents[i].ID;
        }
        return result;
    }

    public int GetWorldEventSize() { return worldEvents.Count; }
}
