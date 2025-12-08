using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInputs : MonoBehaviour {
    public static UserInputs Instance;

    // Player
    public Vector2 playerMove { get; private set; }
    public Vector2 playerLook { get; private set; }
    public bool playerInteract { get; private set; }

    //SongWheel
    public Vector2 songWheelMouseLook { get; private set; }

    public Vector2 songWheelControllerLook { get; private set; }

    public bool openSongWheelPressed { get; private set; }
    public bool openSongWheelReleased { get; private set; }
    public bool songSelect { get; private set; }

    // Journal
    public bool openJournal { get; private set; }
    public bool nextPage { get; private set; }
    public bool previousPage { get; private set; }

    //Notes
    public bool activateNoteSheetPressed { get; private set; }
    public bool activateNoteSheetReleased { get; private set; }
    public bool attributeLeft { get; private set; }
    public bool attributeDown { get; private set; }
    public bool attributeRight { get; private set; }
    public bool attributeUp { get; private set; }
    public bool devButton1 { get; private set; }
    public bool devButton2 { get; private set; }

    [HideInInspector] public InputAction _playerMove;
    private InputAction _playerLook;
    [HideInInspector] public InputAction _playerInteract;

    [HideInInspector] public InputAction _openSongWheel;
    [HideInInspector] public InputAction _songWheelMouseLook;
    [HideInInspector] public InputAction _songWheelControllerLook;
    [HideInInspector] public InputAction _songSelect;

    [HideInInspector] public InputAction _openJournal;
    [HideInInspector] public InputAction _nextPage;
    [HideInInspector] public InputAction _previousPage;

    [HideInInspector] public InputAction _activateNoteSheet;
    [HideInInspector] public InputAction _attributeLeft;
    [HideInInspector] public InputAction _attributeDown;
    [HideInInspector] public InputAction _attributeRight;
    [HideInInspector] public InputAction _attributeUp;

    [HideInInspector] public InputAction _devButton1;
    [HideInInspector] public InputAction _devButton2;

    [HideInInspector] public InputAction _pausMenu;
    [HideInInspector] public InputAction _backPausMenu;

    [SerializeField] private InputActionAsset gameInputActionAsset;
    private InputActionMap _playerInputMap;
    private InputActionMap _songWheelInputMap;
    private InputActionMap _UIInputMap;


    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);

        _songWheelInputMap = gameInputActionAsset.FindActionMap("SongWheel");
        _playerInputMap = gameInputActionAsset.FindActionMap("Player");
        _UIInputMap = gameInputActionAsset.FindActionMap("UI");

        SetUpUserInputs();
    }

    private void Update() {
        UpdateInputs();
    }


    private void SetUpUserInputs() {
        gameInputActionAsset.Enable();
        _songWheelInputMap.Enable();
        _playerInputMap.Enable();
        _UIInputMap.Enable();

        _playerMove = _playerInputMap.FindAction("Move"); // same as move song
        _playerLook = _playerInputMap.FindAction("Look"); // same as look song
        _playerInteract = _playerInputMap.FindAction("Interact"); // Same as select song

        _openJournal = _songWheelInputMap.FindAction("OpenJournal");
        _nextPage = _songWheelInputMap.FindAction("Next");
        _previousPage = _songWheelInputMap.FindAction("Previous");

        _openSongWheel = _songWheelInputMap.FindAction("SongWheel"); // same as move player
        _songWheelMouseLook = _songWheelInputMap.FindAction("MouseLook"); // Same as look player
        _songWheelControllerLook = _songWheelInputMap.FindAction("ControllerLook");
        _songSelect = _songWheelInputMap.FindAction("Select"); // Same as interact player

        _activateNoteSheet = _playerInputMap.FindAction("ActivateNoteSheet");
        _attributeLeft = _playerInputMap.FindAction("AttributeLeft");
        _attributeDown = _playerInputMap.FindAction("AttributeDown");
        _attributeRight = _playerInputMap.FindAction("AttributeRight");
        _attributeUp = _playerInputMap.FindAction("AttributeUp");

        _devButton1 = _playerInputMap.FindAction("DevButton1");
        _devButton2 = _playerInputMap.FindAction("DevButton2");

        _pausMenu = _UIInputMap.FindAction("PausMenu");
        _backPausMenu = _UIInputMap.FindAction("Back");

    }

    private void UpdateInputs() {
        // Player
        playerMove = _playerMove.ReadValue<Vector2>();
        playerLook = _playerLook.ReadValue<Vector2>();
        playerInteract = _playerInteract.WasPressedThisFrame();

        //SongWheel
        songWheelMouseLook = _songWheelMouseLook.ReadValue<Vector2>();
        songWheelControllerLook = _songWheelControllerLook.ReadValue<Vector2>();
        openSongWheelPressed = _openSongWheel.WasPressedThisFrame();
        openSongWheelReleased = _openSongWheel.WasReleasedThisFrame();
        songSelect = _songSelect.WasPressedThisFrame();

        // Journal
        openJournal = _openJournal.WasPressedThisFrame();
        nextPage = _nextPage.WasPressedThisFrame();
        previousPage = _previousPage.WasPressedThisFrame();

        //Attribute Notesheet Interactions
        activateNoteSheetPressed = _activateNoteSheet.WasPressedThisFrame();
        activateNoteSheetReleased = _activateNoteSheet.WasReleasedThisFrame();
        attributeLeft = _attributeLeft.WasPressedThisFrame();
        attributeDown = _attributeDown.WasPressedThisFrame();
        attributeRight = _attributeRight.WasPressedThisFrame();
        attributeUp = _attributeUp.WasPressedThisFrame();

        //Dev Buttons
        devButton1 = _devButton1.WasPressedThisFrame();
        devButton2 = _devButton2.WasPressedThisFrame();
    }

    public void OnOpenNoteSheet() {
        _openSongWheel.Disable();
        _playerInteract.Disable();
    }

    public void OnCloseNoteSheet() {
        if(Player.Instance.isOutOfBody) return;
        _openSongWheel.Enable();
        _playerInteract.Enable();
    }

    public void OnOpenJournal() {
        _playerInputMap.Disable();
        
        _activateNoteSheet.Disable();
        
        _openSongWheel.Disable();
        _songSelect.Disable();
        _songWheelControllerLook.Disable();
        _songWheelMouseLook.Disable();
    }

    public void OnCloseJournal() {
        StartCoroutine(DelayWhenClosingJournal());
    }

    public void OnOpenSongWheel() {
        _playerInputMap.Disable();
        
        _openJournal.Disable();
        _nextPage.Disable();
        _previousPage.Disable();
    }

    public void OnCloseSongWheel() {
        _playerInputMap.Enable();
        
        _openJournal.Enable();
        _nextPage.Enable();
        _previousPage.Enable();
    }

    public void OnMindControlStart()
    {
        _songWheelInputMap.Disable();

        _openSongWheel.Disable();
        _songSelect.Disable();
        _songWheelControllerLook.Disable();
        _songWheelMouseLook.Disable();

        _openJournal.Disable();
        _nextPage.Disable();
        _previousPage.Disable();

        _playerInteract.Enable();
    }

    public void OnMindControlEnd()
    {
        _songWheelInputMap.Enable();

        _openSongWheel.Enable();
        _songSelect.Enable();
        _songWheelControllerLook.Enable();
        _songWheelMouseLook.Enable();

        _openJournal.Enable();
        _nextPage.Enable();
        _previousPage.Enable();
    }

    public void OpenPausMenu() {
        _songWheelInputMap.Disable();
        _playerInputMap.Disable();
    }

    public void ClosePausMenu() {
        _songWheelInputMap.Enable();
        _playerInputMap.Enable();
    }


    public void OnSpellActive()
    {
        _attributeLeft.Disable();
        _attributeDown.Disable();
        _attributeRight.Disable();
        _attributeUp.Disable();
        _activateNoteSheet.Disable();
    }

    public void OnSpellFinished()
    {
        _attributeLeft.Enable();
        _attributeDown.Enable();
        _attributeRight.Enable();
        _attributeUp.Enable();
        _activateNoteSheet.Enable();
    }

    private IEnumerator DelayWhenClosingJournal() {
        yield return new WaitForSeconds(.8f);
        
        _playerInputMap.Enable();

        _activateNoteSheet.Enable();
        
        _openSongWheel.Enable();
        _songSelect.Enable();
        _songWheelControllerLook.Enable();
        _songWheelMouseLook.Enable();
    }
}