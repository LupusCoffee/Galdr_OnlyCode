using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class UIPromptCanvas : MonoBehaviour
{
    public static UIPromptCanvas Instance;
    public UnityEvent _updatePromptButton;
    
    [Header("Components")]
    public CanvasGroup canvasGroup;
    public GameObject messagePrompt;
    public GameObject keybindingMoveMessagePrompt;
    public GameObject keybindingLookMessagePrompt;
    public TextMeshProUGUI header;
    public TextMeshProUGUI message;
    
    [Header("Move")]
    public GameObject moveKeyKeyboardUp;
    public GameObject moveKeyKeyboardDown;
    public GameObject moveKeyKeyboardLeft;
    public GameObject moveKeyKeyboardRight;
    public GameObject moveKeyGamepad;
    
    [Header("Look")]
    public GameObject lookKeyKeyboard;
    public GameObject lookKeyGamepad;
    
    private float _desiredAlpha;
    private float _currentAlpha;
    private ControllerHelper _controllerHelper;
    
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
        
        _currentAlpha = 0;
        _desiredAlpha = 0;
        _updatePromptButton?.Invoke();

    }
    
    private void Start() {
        _controllerHelper = FindFirstObjectByType<ControllerHelper>();
        _updatePromptButton?.Invoke();

    }
    
    void Update() {
        _currentAlpha = Mathf.MoveTowards(_currentAlpha, _desiredAlpha, 2.0f * Time.deltaTime);
        canvasGroup.alpha = _currentAlpha;
    }
    
    public void ShowMessagePrompt(string header, string message) {
        this.header.text = header;
        this.message.text = message;
        
        keybindingMoveMessagePrompt.SetActive(false);
        keybindingLookMessagePrompt.SetActive(false);
        messagePrompt.SetActive(true);
        
        _updatePromptButton?.Invoke();
        _desiredAlpha = 1;
    }
    
    public void ShowMovePrompt() {
        
        if (_controllerHelper != null) {
            if (_controllerHelper.isSwitchController || _controllerHelper.isXboxController ||
                _controllerHelper.isPSController) {
                moveKeyKeyboardDown.SetActive(false);
                moveKeyKeyboardUp.SetActive(false);
                moveKeyKeyboardRight.SetActive(false);
                moveKeyKeyboardLeft.SetActive(false);
                moveKeyGamepad.SetActive(true);
            }
            else {
                moveKeyKeyboardDown.SetActive(true);
                moveKeyKeyboardUp.SetActive(true);
                moveKeyKeyboardRight.SetActive(true);
                moveKeyKeyboardLeft.SetActive(true);
                moveKeyGamepad.SetActive(false);
            }
        }
        _updatePromptButton?.Invoke();
        
        keybindingMoveMessagePrompt.SetActive(true);
        keybindingLookMessagePrompt.SetActive(false);
        messagePrompt.SetActive(false);
        
        _desiredAlpha = 1;
    }
    
    public void ShowLookPrompt() {
        
        if (_controllerHelper != null) {
            if (_controllerHelper.isSwitchController || _controllerHelper.isXboxController ||
                _controllerHelper.isPSController) {
                lookKeyGamepad.SetActive(true);
                lookKeyKeyboard.SetActive(false);
            }
            else {
                lookKeyGamepad.SetActive(false);
                lookKeyKeyboard.SetActive(true);
            }
        }
        _updatePromptButton?.Invoke();
        
        keybindingMoveMessagePrompt.SetActive(false);
        keybindingLookMessagePrompt.SetActive(true);
        messagePrompt.SetActive(false);
        
        _desiredAlpha = 1;
    }

    /// <summary>
    /// Fades the prompt out
    /// </summary>
    public void HidePrompt() {
        _desiredAlpha = 0;
    }
}
