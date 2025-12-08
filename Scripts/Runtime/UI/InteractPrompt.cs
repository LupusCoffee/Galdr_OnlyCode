// Made by Isabelle H. Heiskanen
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractPrompt : MonoBehaviour
{
    public static InteractPrompt Instance;

    public UnityEvent _updatePromptButton;

    [Header("Components")]
    public CanvasGroup canvasGroup;
    public Image promptImage;
    public GameObject interactKeyboard;
    public GameObject interactGamepad;
    public Sprite spellsSprite;
    public Sprite talkSprite;
    public Sprite artifactSprite;
    public Sprite throwSprite;
    public Sprite pushSprite;
    public Sprite grabSprite;
    public Sprite devourerSprite;
    
    
    private float _desiredAlpha;
    private float _currentAlpha;
    private ControllerHelper _controllerHelper;
    
    /// <summary>
    /// Singelton and sets the canvas alpha to 0
    /// </summary>
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
        
        _currentAlpha = 0;
        _desiredAlpha = 0;
    }

    private void Start() {
        _controllerHelper = FindFirstObjectByType<ControllerHelper>();
        _updatePromptButton?.Invoke();

    }

    /// <summary>
    /// Fades the prompt in and out depending on the desiredAplha
    /// </summary>
    void Update() {

        _currentAlpha = Mathf.MoveTowards(_currentAlpha, _desiredAlpha, 2.0f * Time.deltaTime);
        canvasGroup.alpha = _currentAlpha;

    }

    /// <summary>
    /// Sets the sprite to the item sprite and then fades the prompt in
    /// </summary>
    /// <param name="itemSprite"></param>
    public void ShowInteractPrompt(Sprite itemSprite) {
        if (_controllerHelper != null) {
            if (_controllerHelper.isSwitchController || _controllerHelper.isXboxController ||
                _controllerHelper.isPSController) {
                interactGamepad.SetActive(true);
                interactKeyboard.SetActive(false);
            }
            else {
                interactGamepad.SetActive(false);
                interactKeyboard.SetActive(true);
            }
        }
        
        promptImage.sprite = itemSprite;
        _desiredAlpha = 1;
        _updatePromptButton?.Invoke();
    }

    /// <summary>
    /// Fades the prompt out
    /// </summary>
    public void HideInteractPrompt() {
        _desiredAlpha = 0;
    }
}
