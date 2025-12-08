using UnityEngine;

[RequireComponent(typeof(PuzzleEvent))]
[DefaultExecutionOrder(-2)]
public class Puzzle_ClickSendPuzzleEvent : Interactable
{
    PuzzleEvent puzzleEvent;
    private bool hasBeenPressed = false;

    private InteractionType type = InteractionType.BUTTON; //this is very ugly

    [SerializeField] GameObject buttonCrystal;
    [SerializeField] bool isForTheBigRotateBridge;
    [SerializeField] GameObject promptForBigBridge;
    [SerializeField] SpellCastableObject spellCastableObject;
    [SerializeField] GameObject activatableObject;
    HasUnlockedItemCheck unlockItemCheck;

    Vector3 heightOffset = new Vector3(0, 1.5f, 0);
    Vector3 defaultScale;

    protected override void Start()
    {
        if (promptForBigBridge != null) promptForBigBridge.SetActive(false);
        puzzleEvent = GetComponent<PuzzleEvent>();

        /*if (puzzleEvent != null)
        {
            Debug.Log("Puzzle Event retrieved.");
        }*/
        //HasUnlockedItemCheck unlockItemCheck = null;
        if (activatableObject != null) {
            unlockItemCheck = activatableObject.GetComponent<HasUnlockedItemCheck>();
            //this.activatableObject.SetActive(false); //probably needs to be changed??
        }

        if (unlockItemCheck != null) {
            if (Inventory.HasUnlockedItem(unlockItemCheck.Id)) {
                //Debug.Log("Item already retrieved.");
                this.SetButtonActive(false);
            } else {
                //Debug.Log("Item not retrieved.");
                this.SetButtonInactive();
            } 
        } else {
            //Debug.Log("No HasUnlockedItemCheck script.");
            this.SetButtonInactive();
        }
    }

    private void Update() {
        if (this.hasBeenPressed) {
            this.buttonCrystal.transform.Rotate(Vector3.up, 1);
        }
    }

    public override SO_InteractableData Interact() {
        if (this.hasBeenPressed) return null;

        this.SetButtonActive(true);
        //Debug.Log($"[Puzzle_ClickSendPuzzleEvent] hasBeenPressed is set to {this.hasBeenPressed}.");
        if (activatableObject != null) {
            unlockItemCheck = activatableObject.GetComponent<HasUnlockedItemCheck>();
        }

        if (unlockItemCheck != null) {
            if (!Inventory.HasUnlockedItem(unlockItemCheck.Id)) {
                Debug.Log("Spawning Item from Button press.");
                //this.activatableObject.SetActive(true);
            }
        }

        if (isForTheBigRotateBridge) {
            this.promptForBigBridge.SetActive(true);
            this.spellCastableObject.enabled = true;
        }

        return null;
    }

    private void SetButtonInactive() {
        this.buttonCrystal.SetActive(false);
        this.buttonCrystal.transform.position = transform.position;
        this.defaultScale = buttonCrystal.transform.localScale;
    }

    private void SetButtonActive(bool justPressed) {
        this.hasBeenPressed = true;

        if (justPressed) {
            this.puzzleEvent.ButtonPress();
            this.buttonCrystal.SetActive(true);
            LeanTween.move(buttonCrystal, transform.position + new Vector3(0, 1.5f, 0), 1f).setEaseInOutCubic();
        } else {
            //puzzleEvent.ButtonPress();
            this.buttonCrystal.SetActive(true);
            LeanTween.move(buttonCrystal, transform.position + new Vector3(0, 1.5f, 0), 1f).setEaseInOutCubic(); //remove lean and just set the position?
        }
    }

    public InteractionType GetButtonInteractionType() { return this.type; }

    public bool IsButtonActive() {
        /*if (!this.hasBeenPressed) { return true; }
        return false;*/
        return this.hasBeenPressed;
    }
}
