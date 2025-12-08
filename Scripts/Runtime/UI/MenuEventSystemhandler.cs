using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuEventSystemhandler : MonoBehaviour {
    
    [Header("Reference")] 
    public List<Selectable> selectables = new List<Selectable>();
    [SerializeField] protected Selectable _firstSelectable;

    [Header("Navigation")] 
    [SerializeField] protected InputActionReference _navigationReference;

    [Header("Animations")] 
    [SerializeField] protected float _selectedAnimationScale = 1.1f;
    [SerializeField] protected float _scaleDuration = 0.25f;
    [SerializeField] protected List<GameObject> _animationExclusive = new List<GameObject>();

    [Header("Sounds")] 
    [SerializeField] protected UnityEvent _soundEvent;

    protected Dictionary<Selectable, Vector3> _scales = new Dictionary<Selectable, Vector3>();

    protected Selectable _lastSelectable;

    private void OnEnable() {

        _navigationReference.action.performed += OnNavigate;
        
        // Reset all scales to original size
        for (int i = 0; i < selectables.Count; i++) {
            selectables[i].transform.localScale = _scales[selectables[i]];
        }

        StartCoroutine(SelectAfterDelay());
    }

    private void OnDisable() {
        _navigationReference.action.performed -= OnNavigate;
    }

    protected virtual IEnumerator SelectAfterDelay() {
        yield return null;
        EventSystem.current.SetSelectedGameObject(_firstSelectable.gameObject);
    }

    public void Awake() {
        LeanTween.reset();
        
        foreach (var selectable in selectables) {
           AddSelectionListeners(selectable); 
           _scales.Add(selectable, selectable.transform.localScale);
        }
    }

    protected virtual void AddSelectionListeners(Selectable selectable) {
        // Add listeners
        EventTrigger trigger = selectable.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) {
            trigger = selectable.gameObject.AddComponent<EventTrigger>();
        }
        
        // Add SELECT event
        EventTrigger.Entry SelectEntry = new EventTrigger.Entry {
            eventID = EventTriggerType.Select
        };
        SelectEntry.callback.AddListener(OnSelect);
        trigger.triggers.Add(SelectEntry);

        // Add DESELECT event
        EventTrigger.Entry DeselectEntry = new EventTrigger.Entry {
            eventID = EventTriggerType.Deselect
        };
        DeselectEntry.callback.AddListener(OnDeselect);
        trigger.triggers.Add(DeselectEntry);
        
        // Add POINTER ENTER event
        EventTrigger.Entry PointerEnter = new EventTrigger.Entry {
            eventID = EventTriggerType.PointerEnter
        };
        PointerEnter.callback.AddListener(OnPointerEnter);
        trigger.triggers.Add(PointerEnter);
        
        // Add ON POINTER EXIT event
        EventTrigger.Entry PointerExit = new EventTrigger.Entry {
            eventID = EventTriggerType.PointerExit
        };
        PointerExit.callback.AddListener(OnPointerExit);
        trigger.triggers.Add(PointerExit);

    }

    public void OnSelect(BaseEventData eventData) {
        _soundEvent?.Invoke();
        _lastSelectable = eventData.selectedObject.GetComponent<Selectable>();

        if (_animationExclusive.Contains(eventData.selectedObject))
            return;
        
        Vector3 newScale = eventData.selectedObject.transform.localScale * _selectedAnimationScale;
        LeanTween.scale(eventData.selectedObject, newScale, _scaleDuration);
    }
    
    public void OnDeselect(BaseEventData eventData) {
        if (_animationExclusive.Contains(eventData.selectedObject))
            return;
        
        Selectable selectable = eventData.selectedObject.GetComponent<Selectable>();
        LeanTween.scale(eventData.selectedObject, _scales[selectable], _scaleDuration);
    }
    
    public void OnPointerEnter(BaseEventData eventData) {
        PointerEventData pointerEventData = eventData as PointerEventData;
        if (pointerEventData != null) {
            Selectable selectable = pointerEventData.pointerEnter.GetComponentInParent<Selectable>();
            if (selectable == null) {
                selectable = pointerEventData.pointerEnter.GetComponentInChildren<Selectable>();
            }

            pointerEventData.selectedObject = selectable.gameObject;
        }
    }
    
    public void OnPointerExit(BaseEventData eventData) {
        PointerEventData pointerEventData = eventData as PointerEventData;
        if (pointerEventData != null) {
            pointerEventData.selectedObject = null;
        }
    }

    protected virtual void OnNavigate(InputAction.CallbackContext context) {
        if (EventSystem.current.currentSelectedGameObject == null && _lastSelectable != null) {
            EventSystem.current.SetSelectedGameObject(_lastSelectable.gameObject);
        }
    }

    public void SetFirstSelected() {
        EventSystem.current.SetSelectedGameObject(_firstSelectable.gameObject);

    }

    public void SetNewFirstSelected(GameObject newFirstSelected) {
        EventSystem.current.SetSelectedGameObject(newFirstSelected);
    }

    public void RemoveFromSelectable(Selectable selectable) {
        selectables.Remove(selectable);
    }

    public void AddToSelectable(Selectable selectable) {
        selectables.Add(selectable);
    }

}