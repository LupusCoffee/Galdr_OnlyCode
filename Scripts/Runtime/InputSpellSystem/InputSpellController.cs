using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using static AbilityBehaviour;
using static MultiTag;
using static SpellTargetFinder;

//note for future polish:
//- make this a state machine: 1. spell effect choosing, 2. spell target choosing
//- make it so you can choose any spell effect you have unlocked,
//  so that when you look at an object, it takes the spell target from it (if they are compatible)
//  and shows the note sequence for the spell target. This way we can actually combine spells meaningfully.
//- also refactor/rename some fucntions and optimize a bit
public class InputSpellController : MonoBehaviour
{
    //General
    private bool spellControllerActive = false;
    
    public bool SpellControllerActive { get => spellControllerActive; }
    private Camera camera; //will be used later!!!!
    private Transform playerTransform;
    
    private UiManager uiManager;
    
    private ProximityHelper proximityHelper = new();
    private HighlightHelper highlightHelper = new();
    
    //Spell Progress & Chosen Spell
    private enum SpellProgress { EFFECT, TARGET, ACTIVATE } //as it is right now, effect and target are more like "choose spell" and "input note sequence"
    private SpellProgress spellProgress = 0;
    
    private AbilityBehaviour abilityBehaviour = new(); //used to activate an ability based on spell effect and spell target enums
    private AbilityBehaviour.SpellEffect chosenSpellEffectEnum;
    private AbilityBehaviour.SpellTarget chosenSpellTargetEnum;
    
    private List<InputSpellNote> spellTargetNoteSequenceToMatch;
    private int spellTargetNoteSequenceToMatchProgress = 0;
    
    //Spell Interactable Object Handling
    [Header("Chosen Spell Parameters")]
    [SerializeField]
    private float chosenForwardRadius = 2f;
    
    [SerializeField]
    private float chosenForwardDistance = 10f;
    
    [SerializeField]
    private float chosenSurroundingRadius = 1.5f;
    
    [SerializeField]
    private float chosenSurroundingDistance = 2f;
    
    [Header("Let-Go Spell Parameters")]
    [SerializeField]
    private float letGoForwardRadius = 2f;
    
    [SerializeField]
    private float letGoForwardDistance = 10f;
    
    [SerializeField]
    private float letGoSurroundingRadius = 7f;
    
    [SerializeField]
    private float letGoSurroundingDistance = 2f;
    
    [Header("Misc")]
    [SerializeField]
    private LayerMask spellCastableLayerMask;
    
    private List<SpellCastableObject> spellCastableObjectsInProximity = new(); //multitags, outlineobjects
    private List<GameObject> spellCastableObjectGameObjectsInProximity = new(); //for counting without having to do get component unnecessarily
    private SpellCastableObject closestSpellCastableObjectOfChosenSpellEffect = null;
    private GameObject closestSpellCastableObjectGameObjectOfChosenSpellEffect = null;
    
    //UI
    private float _desiredAlpha = 0;
    private float _currentAlpha = 0;
    

    #region Input Setup
    private void OnEnable()
    {
        UserInputs.Instance._activateNoteSheet.performed += OnToggleNoteSheet;
        UserInputs.Instance._attributeLeft.performed += OnAttributeLeft;
        UserInputs.Instance._attributeDown.performed += OnAttributeDown;
        UserInputs.Instance._attributeRight.performed += OnAttributeRight;
        UserInputs.Instance._attributeUp.performed += OnAttributeUp;
    }
    private void OnDisable()
    {
        UserInputs.Instance._activateNoteSheet.performed -= OnToggleNoteSheet;
        UserInputs.Instance._attributeLeft.performed -= OnAttributeLeft;
        UserInputs.Instance._attributeDown.performed -= OnAttributeDown;
        UserInputs.Instance._attributeRight.performed -= OnAttributeRight;
        UserInputs.Instance._attributeUp.performed -= OnAttributeUp;
    }
    #endregion
    
    
    private void Start()
    {
        camera = Camera.main;
        playerTransform = Player.Instance.transform;
        
        uiManager = UiManager.Instance;
    }

    
    #region Button Inputs
    private void OnToggleNoteSheet(InputAction.CallbackContext callbackContext)
    {
        ToggleNoteSheet();
        uiManager.ToggleFaceButtonIcons(FaceButtonsState.SpellEffectAndInput);
    }
    private void OnAttributeDown(InputAction.CallbackContext callbackContext)
    {
        ButtonPress(InputSpellInput.SOUTH, callbackContext);
    }
    private void OnAttributeRight(InputAction.CallbackContext callbackContext)
    {
        ButtonPress(InputSpellInput.EAST, callbackContext);
    }
    private void OnAttributeUp(InputAction.CallbackContext callbackContext)
    {
        ButtonPress(InputSpellInput.NORTH, callbackContext);
    }
    private void OnAttributeLeft(InputAction.CallbackContext callbackContext)
    {
        ButtonPress(InputSpellInput.WEST, callbackContext);
    }
    #endregion
    
    
    #region Button Presses and Update
    private void ToggleNoteSheet()
    {
        if (spellControllerActive) //close note sheet
        {
            //actual logic
            Player.Instance.SetIsPlayingMusic(false);
            spellControllerActive = false;
            
            //UI / Highlight / VFX
            _desiredAlpha = 0;
            highlightHelper.DisableHighlightOnAllObjects();
            //stop playing spell vfx
            
            //misc
            UserInputs.Instance.OnCloseNoteSheet();
        }
        else //open note sheet
        {
            //actual logic
            if (Player.Instance.isOutOfBody) return;
            Player.Instance.SetIsPlayingMusic(true);
            spellControllerActive = true;
            highlightHelper.ReapplyHighlights();
            SetSpellProgress(SpellProgress.EFFECT);
            
            //UI / Highlight / VFX
            _desiredAlpha = 1;
            uiManager.ToggleFaceButtonIcons(FaceButtonsState.SpellEffectAndInput);
            uiManager.DisplayActiveSpellEffect(false);
            //start playing spell vfx
            
            //misc
            UserInputs.Instance.OnOpenNoteSheet();
        }
    }
    
    private void ButtonPress(InputSpellInput input, InputAction.CallbackContext callbackContext) //todo: state machine god damn it!!!
    {
        if (!spellControllerActive) return;

        if (spellProgress == SpellProgress.EFFECT)
        {
            SpellEffectButtonPress(input);
            return;
        }
        if (spellProgress == SpellProgress.TARGET)
        {
            SpellTargetButtonPress(input);
        }
    }
    private void SpellEffectButtonPress(InputSpellInput input) //more like choose spell
    {
        if (!TryChooseSpellEffect(input, out var spellEffectSO)) return;
        chosenSpellEffectEnum = spellEffectSO.GetSpellEffectEnum();
        if (chosenSpellEffectEnum == SpellEffect.NONE) return;

        UpdateClosestSpellCastableObjectOfChosenSpellEffectInProximity();
        
        if (!TryGetChosenSpellTarget(out var spellTargetSO)) return;
        chosenSpellTargetEnum = spellTargetSO.GetSpellTargetEnum();
        if (chosenSpellTargetEnum == AbilityBehaviour.SpellTarget.NONE) return;
        if (!TrySetSpellTargetNoteSequence(spellTargetSO)) return;
        
        //audio visuals
        MusicManager.Instance.PostEvent(spellEffectSO.GetSpellEffectSound());
        uiManager.SetActiveSpellEffectIcon(input);
        uiManager.PopulateBarLayoutGroups(spellTargetSO.GetNoteSequence());
        
        SetSpellProgress(SpellProgress.TARGET);
    }
    private void SpellTargetButtonPress(InputSpellInput input)
    {
        InputSpellNote note = spellTargetNoteSequenceToMatch[spellTargetNoteSequenceToMatchProgress];

        if (note.GetInputButton() == input)
        {
            MusicManager.Instance.PostEvent(note.GetSound());

            int barNum = spellTargetNoteSequenceToMatchProgress;
            uiManager.ShowNoteInRegularBarLayoutGroup(barNum, note);
            
            spellTargetNoteSequenceToMatchProgress++;
        }
        else
        {
            spellTargetNoteSequenceToMatchProgress = 0;
            uiManager.DisableAllNotesInRegularBarLayoutGroup();
            return;
        }

        if (spellTargetNoteSequenceToMatch.Count == spellTargetNoteSequenceToMatchProgress)
        {
            abilityBehaviour.Activate(chosenSpellEffectEnum, chosenSpellTargetEnum, closestSpellCastableObjectOfChosenSpellEffect.gameObject);
            ToggleNoteSheet();
        }
    }
    
    private void Update()
    {
        if (spellControllerActive) UpdateSpellController();
        
        //UI - Input Spell Controller Canvas
        _currentAlpha = Mathf.MoveTowards(_currentAlpha, _desiredAlpha, 5 * Time.deltaTime);
        UiManager.Instance.GetInputSpellControllerCanvas().alpha = _currentAlpha;
    }
    private void UpdateSpellController()  //todo: do every 0.1 sec, not every tick
    {
        if (spellProgress == SpellProgress.EFFECT)
            UpdateSpellEffectState();
        if(spellProgress == SpellProgress.TARGET)
            UpdateSpellTargetState();
    }
    private void UpdateSpellEffectState()
    {
        //all of this just updates the ui and highlights - move? rename? hrmmm
        
        UpdateAllSpellEffectRunTimeDataAvailability();
        
        List<SpellEffectRunTimeData> spellEffectRunTimeDatas = SpellManager.Instance.GetAllUnlockedSpellEffectRunTimeDatas();
        uiManager.UpdateAllSpellEffectFaceButtonIconsAvailability(spellEffectRunTimeDatas);
        
        highlightHelper.UpdateHighlightsOnMultipleObjects(spellCastableObjectsInProximity);
    }
    private void UpdateSpellTargetState()
    {
        if (!IsClosestSpellCastableGameObjectStillInProximity())
        {
            SetSpellProgress(SpellProgress.EFFECT);
            return;
        }
        
        //Highlighting
        highlightHelper.UpdateHighlightOnSingleObject(closestSpellCastableObjectOfChosenSpellEffect);
    }
    
    private void SetSpellProgress(SpellProgress _spellProgress)
    {
        spellProgress = _spellProgress;
        
        if (spellProgress == SpellProgress.EFFECT)
        {
            uiManager.ToggleFaceButtonIcons(FaceButtonsState.SpellEffectAndInput);
            uiManager.DisplayActiveSpellEffect(false);
            uiManager.DisplayBarLayoutGroups(false);
        }

        if (spellProgress == SpellProgress.TARGET)
        {
            uiManager.ToggleFaceButtonIcons(FaceButtonsState.InputOnly);
            uiManager.DisplayActiveSpellEffect(true);
            uiManager.DisplayBarLayoutGroups(true);
        }
    }
    #endregion
    
    
    #region Spell Effect Methods
    private bool TryChooseSpellEffect(InputSpellInput input, out SO_SpellPart_Effect spellEffectSO)
    {
        spellEffectSO = null;
        
        if (!SpellManager.Instance.TryGetUnlockedSpellEffectRunTimeData(input, out var spellEffectRunTimeData)) return false;
        
        UpdateSpecificSpellEffectAvailability(spellEffectRunTimeData);
        if (!spellEffectRunTimeData.IsAvailable()) return false;
        
        if (!spellEffectRunTimeData.TryGetSpellEffectSO(out var _spellEffectSO)) return false;
        spellEffectSO = _spellEffectSO;
        
        return true;
    }
    private void UpdateSpecificSpellEffectAvailability(SpellEffectRunTimeData spellEffectRunTimeData)
    {
        UpdateSpellCastableMultiTagsAndObjectsInProximity(); //todo: wanna access this from SpellTargetFinder instead > so move it!!

        List<MultiTag> spellCastablMultiTagsInProximity = new(); //todo: wanna get this from spell target finder
        foreach (var inputSpellObjectData in spellCastableObjectsInProximity)
        {
            if(!inputSpellObjectData.TryGetMultiTag(out var multiTag)) continue;
            spellCastablMultiTagsInProximity.Add(multiTag);
        }
        
        spellEffectRunTimeData.UpdateAvailabilityBasedOnMultiTags(spellCastablMultiTagsInProximity);
    }
    private void UpdateAllSpellEffectRunTimeDataAvailability()
    {
        UpdateSpellCastableMultiTagsAndObjectsInProximity();
        
        List<MultiTag> spellCastableMultiTagsInProximity = new(); //todo: wanna get this from spell target finder
        foreach (var inputSpellObjectData in spellCastableObjectsInProximity)
        {
            if(!inputSpellObjectData.TryGetMultiTag(out var multiTag)) continue;
            spellCastableMultiTagsInProximity.Add(multiTag);
        }
        
        List<SpellEffectRunTimeData> spellEffectRunTimeDatas = SpellManager.Instance.GetAllUnlockedSpellEffectRunTimeDatas();
        foreach (var spellEffectRunTimeData in spellEffectRunTimeDatas)
            spellEffectRunTimeData.UpdateAvailabilityBasedOnMultiTags(spellCastableMultiTagsInProximity);
    }
    private void UpdateSpellCastableMultiTagsAndObjectsInProximity() //note: this method keeps the lists synced - multitags and objects
    {
        List<GameObject> currentFrameSpellCastableGameObjectsInProximity = GetGameObjectsWithSpellCastableLayerInProximity(
            chosenForwardRadius, chosenForwardDistance, chosenSurroundingRadius, chosenSurroundingDistance);
        
        //sync last frame to current frame
        for (int i = currentFrameSpellCastableGameObjectsInProximity.Count - 1; i >= 0; i--)
        {
            GameObject hitObject = currentFrameSpellCastableGameObjectsInProximity[i]; //todo: rename from hit object to something else
            if (spellCastableObjectGameObjectsInProximity.Contains(hitObject)) continue;

            if (!hitObject.TryGetComponent<SpellCastableObject>(out var spellCastableObject)) continue;
            spellCastableObjectsInProximity.Add(spellCastableObject);
            spellCastableObjectGameObjectsInProximity.Add(hitObject);
        }
        for (int i = spellCastableObjectGameObjectsInProximity.Count - 1; i >= 0; i--)
        {
            GameObject lastUpdateGameObject = spellCastableObjectGameObjectsInProximity[i];
            if (currentFrameSpellCastableGameObjectsInProximity.Contains(spellCastableObjectGameObjectsInProximity[i])) continue;
                
            spellCastableObjectsInProximity.RemoveAt(i);
            spellCastableObjectGameObjectsInProximity.RemoveAt(i);
        }
    }
    private void UpdateClosestSpellCastableObjectOfChosenSpellEffectInProximity()
    {
        closestSpellCastableObjectOfChosenSpellEffect = GetClosestSpellCastableObjectOfChosenSpellEffectEnum();
        
        closestSpellCastableObjectGameObjectOfChosenSpellEffect = closestSpellCastableObjectOfChosenSpellEffect.gameObject;
    }

    private SpellCastableObject GetClosestSpellCastableObjectOfChosenSpellEffectEnum()
    {
        SpellCastableObject closestSpellCastableObject = null;
        float closestDistance = float.MaxValue;
        
        foreach (var spellCastableObject in spellCastableObjectsInProximity)
        {
            //if not fits with the spell effect enum, continue
            if (!spellCastableObject.TryGetMultiTag(out var multiTag)) continue;
            if (chosenSpellEffectEnum != SpellManager.Instance.GetSpellEffectEnumByMultiTag(multiTag)) continue;
            
            float distanceToTarget = Vector3.Distance(playerTransform.position, spellCastableObject.transform.position);

            if (closestDistance < distanceToTarget) continue;
            
            closestSpellCastableObject = spellCastableObject;
            closestDistance = distanceToTarget;
        }
        
        return closestSpellCastableObject;
    }
    #endregion
    
    
    #region Spell Target Methods
    private bool TryGetChosenSpellTarget(out SO_SpellPart_Target spellTargetSO)
    {
        spellTargetSO = null;
        
        if (!closestSpellCastableObjectOfChosenSpellEffect.TryGetMultiTag(out var multiTag)) return false;
        if (!SpellManager.Instance.TryGetUnlockedSpellTarget(multiTag, out var target)) return false;
        spellTargetSO = target;
        return true;
    }
    private bool TrySetSpellTargetNoteSequence(SO_SpellPart_Target target)
    {
        spellTargetNoteSequenceToMatch = target.GetNoteSequence();
        if (spellTargetNoteSequenceToMatch.Count == 0)
        {
            spellTargetNoteSequenceToMatch = null;
            return false;
        }
        
        spellTargetNoteSequenceToMatchProgress = 0;
        return true;
    }
    private bool IsClosestSpellCastableGameObjectStillInProximity()
    {
        List<GameObject> _spellCastableObjectGameObjectsInProximity = GetGameObjectsWithSpellCastableLayerInProximity(
                letGoForwardRadius, letGoForwardDistance, letGoSurroundingRadius, letGoSurroundingDistance);

        if (!_spellCastableObjectGameObjectsInProximity.Contains(closestSpellCastableObjectGameObjectOfChosenSpellEffect)) return false;
        return true;
    }
    #endregion
    

    #region Misc Methods
    private List<GameObject> GetGameObjectsWithSpellCastableLayerInProximity(float forwardRadius, float forwardDistance, float surroundingRadius, float surroundingDistance)
    {
        Transform origin = playerTransform; //todo: go from camera, not player
        Vector3 originPos = origin.position + new Vector3(0, -0.15f, 0); // Adjusted height
        RaycastHit[] currentFrameHitsForward = Physics.SphereCastAll(originPos, forwardRadius, 
            origin.forward, forwardDistance, spellCastableLayerMask);
        RaycastHit[] currentFrameHitsSurrounding = Physics.SphereCastAll(originPos, surroundingRadius,
            Vector3.down, surroundingDistance, spellCastableLayerMask);
        
        List<GameObject> currentFrameHitGameObjects = new();
        foreach (RaycastHit currentFrameHit in currentFrameHitsForward)
            currentFrameHitGameObjects.Add(currentFrameHit.collider.gameObject);
        foreach (RaycastHit currentFrameHit in currentFrameHitsSurrounding)
        {
            if (!currentFrameHitGameObjects.Contains(currentFrameHit.collider.gameObject))
                currentFrameHitGameObjects.Add(currentFrameHit.collider.gameObject);
        }

        return currentFrameHitGameObjects;
    }
    #endregion
}
