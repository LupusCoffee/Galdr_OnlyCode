using System.Collections;
using UnityEngine;
using static CompactMath;

// Button that only has an activation event, no deactivation event
public class Puzzle_ActivateButton : Interactable {
    public delegate void OnPress(GameObject obj);
    public event OnPress OnButtonPressed;
    private bool hasBeenPressed = false;

    private InteractionType type = InteractionType.BUTTON; //this is very ugly

    [SerializeField] GameObject buttonCrystal;

    public bool isPressable;

    Vector3 heightOffset = new Vector3(0, 1.5f, 0);
    Vector3 defaultScale;

    protected override void Start()
    {
        buttonCrystal.SetActive(false);
        buttonCrystal.transform.position = transform.position;
        defaultScale = buttonCrystal.transform.localScale;
    }

    private void Update()
    {
        if (hasBeenPressed) {
            buttonCrystal.transform.Rotate(Vector3.up, 1);
        }
    }

    public override SO_InteractableData Interact() {
        if (hasBeenPressed || !isPressable) return null;

        hasBeenPressed = true;
        OnButtonPressed?.Invoke(gameObject);

        buttonCrystal.SetActive(true);
        LeanTween.move(buttonCrystal, transform.position + new Vector3(0, 1.5f, 0), 1f).setEaseInOutCubic();

        return null;
    }

    public IEnumerator ResetButton()
    {
        yield return new WaitForSeconds(1F);
        hasBeenPressed = false;
        LeanTween.move(buttonCrystal, transform.position, 0.5f).setEaseInOutCubic();
        LeanTween.rotate(buttonCrystal, Vector3.zero, 0.5f).setEaseInOutCubic();

        yield return new WaitForSeconds(0.5F);
        buttonCrystal.SetActive(false);
    }

    public InteractionType GetButtonInteractionType() { return this.type; }

    public bool IsButtonActive() {
        return (isPressable ^ hasBeenPressed);
    }

    public bool HasBeenPressed() { return hasBeenPressed; }

    private void OnDestroy()
    {
        OnButtonPressed = null;
    }

    public override string ToString() {
        return "Interacatble Puzzle Button";
    }

    public void ShowOrder()
    {
        Debug.Log("Showing order");

        buttonCrystal.SetActive(true);
        buttonCrystal.transform.position = transform.position + heightOffset;
        buttonCrystal.transform.localScale = Vector3.zero;

        LeanTween.scale(buttonCrystal, defaultScale, 0.5f).setEaseInOutCubic().setOnComplete(() =>
        {
            LeanTween.scale(buttonCrystal, Vector3.zero, 0.5f).setEaseInOutCubic().setDelay(0.5f);
        }).setOnComplete(() =>
        {
            buttonCrystal.transform.position = transform.position;
            buttonCrystal.transform.localScale = defaultScale;
            buttonCrystal.SetActive(false);
        });
    }
}
