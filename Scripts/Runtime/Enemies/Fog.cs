using System;
using UnityEngine;

public class Fog : MonoBehaviour {
    private OverlayHandler _overlayHandler;

    private void Start() {
        _overlayHandler = FindFirstObjectByType<OverlayHandler>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            _overlayHandler.ShowOverlay();
        }
    }

    private void OnTriggerExit(Collider other) {
        _overlayHandler.HideOverlay();
    }
}
