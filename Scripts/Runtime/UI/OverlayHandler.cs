// Made by Isabelle H. Heiskanen

using UnityEngine;

public class OverlayHandler : MonoBehaviour
{
    
    public static OverlayHandler Instance;
    
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup canvasGroupMindControl;
    [SerializeField] private GameObject postProcessPurple;
    
    private float _desiredAlphaHurtOverlay;
    private float _currentAlphaHurtOverlay;
    private bool _isShowingHurtOverlay;

    private float timeToDeath = 3;
    private float currentTimer = 0;
    
    private float _desiredAlphaMindControl;
    private float _currentAlphaMindControl;
    
    
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
        
        canvasGroup.alpha = 0;
        canvasGroupMindControl.alpha = 0;
        postProcessPurple.SetActive(false);
    }
    
    void Update()
    {
        // MIND CONTROL ALPHA CONTROLLER
        _currentAlphaMindControl = Mathf.MoveTowards(_currentAlphaMindControl, _desiredAlphaMindControl, 2.0f * Time.deltaTime);
        canvasGroupMindControl.alpha = _currentAlphaMindControl;
        
        // HURT OVERLAY CONTROLLER
        
        if (_isShowingHurtOverlay && currentTimer > 0) {
            currentTimer -= Time.deltaTime;
        }

        if (!_isShowingHurtOverlay) return;
        
        if (currentTimer <= 0) {
            TransitionAnimator.Instance.ResetLevel();
            HideOverlay();
        }
     
    }

    private void ToggleDesiredAlpha() {
        if(Mathf.Approximately(_desiredAlphaHurtOverlay, 1))
            HideOverlay();
        else {
            ShowOverlay();
        }
    }
    
    public void ShowOverlay() {
        currentTimer = timeToDeath;
        _isShowingHurtOverlay = true;
        LeanTween.alphaCanvas(canvasGroup, 1, 0.5f).setLoopPingPong();
    }
    
    public void HideOverlay() {
        _isShowingHurtOverlay = false;
        LeanTween.cancel(canvasGroup.gameObject);
        LeanTween.alphaCanvas(canvasGroup, 0, 0.5f);
    }
  
    // MIND CONTROL OVERLAY HANDLER METHODS
    
    public void ShowOverlayMindControl() {
        _desiredAlphaMindControl = 1;
        postProcessPurple.SetActive(true);
    }
    
    public void HideOverlayMindControl() {
        _desiredAlphaMindControl = 0;
        postProcessPurple.SetActive(false);
    }
    
}
