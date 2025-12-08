using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PickupEventListener : PuzzleEvent
{
    [SerializeField] List<PickupEventSender> senders = new List<PickupEventSender>();

    int linkedPickups = 0;

    int totalPickups = 0;

    bool finished;

    private void Awake()
    {
        PickupEventSender.OnPickup += OnPickup;
    }

    private void Start()
    {
        totalPickups = senders.Count;

        if (senders == null || senders.Count == 0)
        {
            finished = true;
            TriggerOnButtonPressed();
            return;
        }

        foreach (PickupEventSender sender in senders)
        {
            if (sender.TryGetComponent(out HasUnlockedItemCheck itemCheck))
            {
                if (!itemCheck.IsItemActive)
                {
                    finished = true;
                    TriggerOnButtonPressed();
                    return;
                }
            }
        }
    }

    private void OnPickup()
    {
        if (finished) return;

        linkedPickups++;

        if (linkedPickups >= totalPickups)
        {
            Debug.Log("All pickups have been picked up");

            if(ProgressionManager.Instance.currentStage == ProgressionManager.CurrentStage.SearchingForPossessSpell) {
                ProgressionManager.Instance.SetCurrentStage(ProgressionManager.CurrentStage.UnlockingPossessSpell);
            }

            TriggerOnButtonPressed();
        }
    }

    private void OnDestroy()
    {
        PickupEventSender.OnPickup -= OnPickup;
    }
}
