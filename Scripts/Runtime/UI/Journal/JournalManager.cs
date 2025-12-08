// Made by Isabelle H. Heiskanen

using SaveSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class JournalManager : MonoBehaviour {
    public static JournalManager Instance; 

    public UnityEvent _updatePromptButton;


    [Header("Components")] 
    public CanvasGroup canvasGroup;
    public GameObject[] pages;
    public GameObject interactKeyboardPrevious;
    public GameObject interactGamepadPrevious;
    public GameObject interactKeyboardNext;
    public GameObject interactGamepadNext;
    public Sprite tabIn;
    public Sprite tabOut;

    [Header("New Canvas")]
    public CanvasGroup canvasGroupNew;
    public GameObject[] pagesNews;
    public GameObject interactKeyboardPreviousNew;
    public GameObject interactGamepadPreviousNew;
    public GameObject interactKeyboardNextNew;
    public GameObject interactGamepadNextNew;
    public Sprite tabInNew;
    public Sprite tabOutNew;
    public GameObject cinemachineCamera;

    private float _desiredAlpha;
    private float _currentAlpha;
    private bool _isJournalOpen = false;
    private int _activePageIndex = 0;
    private JournalPage _activeJournalPage;
    private ControllerHelper _controllerHelper;

    /// <summary>
    /// Subscribes to the inputs events
    /// </summary>
    private void OnEnable() {
        UserInputs.Instance._openJournal.performed += OpenMenuInput;
        UserInputs.Instance._nextPage.performed += NextInput;
        UserInputs.Instance._previousPage.performed += PreviousInput;
    }

    private void OnDisable() {
        UserInputs.Instance._openJournal.performed -= OpenMenuInput;
        UserInputs.Instance._nextPage.performed -= NextInput;
        UserInputs.Instance._previousPage.performed -= PreviousInput;
    }

    /// <summary>
    /// Singelton and sets the canvas group alpha to 0
    /// </summary>
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);

        _currentAlpha = 0;
        _desiredAlpha = 0;
        //canvasGroup.interactable = false;
        //canvasGroup.blocksRaycasts = false;
        
        canvasGroupNew.interactable = false;
        canvasGroupNew.blocksRaycasts = false;
        
    }

    private void Start() {
        _controllerHelper = FindFirstObjectByType<ControllerHelper>();
        _updatePromptButton?.Invoke();
    }

    /// <summary>
    /// Fades the journal in and out
    /// </summary>
    private void Update() {
        // _currentAlpha = Mathf.MoveTowards(_currentAlpha, _desiredAlpha, 2.0f * Time.deltaTime);
        // canvasGroup.alpha = _currentAlpha;
        
        _currentAlpha = Mathf.MoveTowards(_currentAlpha, _desiredAlpha, 2.0f * Time.deltaTime);
        canvasGroupNew.alpha = _currentAlpha;

        if (_isJournalOpen) {
            ShowCorrectButton();
        }
    }

    /// <summary>
    /// Toggle the journal on or off
    /// </summary>
    private void ToggleJournal() {
        if (!_isJournalOpen) {
            OpenJournal();
        }
        else {
            CloseJournal();
        }
    }


    /// <summary>
    /// Open the journal
    /// </summary>
    private void OpenJournal() {
        _desiredAlpha = 1;
        _isJournalOpen = true;
        GoToFirstPage();
        
        //canvasGroup.interactable = true;
        //canvasGroup.blocksRaycasts = true;
        canvasGroupNew.interactable = true;
        canvasGroupNew.blocksRaycasts = true;
        
        ShowCorrectButton();
        PostProcessingManager.Instance.TurnOnJournal();

        UserInputs.Instance.OnOpenJournal();
        
        cinemachineCamera.SetActive(true);
        UIPromptCanvas.Instance.HidePrompt();
    }

    private void ShowCorrectButton() {
        // if (_controllerHelper != null) {
        //     if (_controllerHelper.isSwitchController || _controllerHelper.isXboxController ||
        //         _controllerHelper.isPSController) {
        //         interactGamepadPrevious.SetActive(true);
        //         interactGamepadNext.SetActive(true);
        //         interactKeyboardPrevious.SetActive(false);
        //         interactKeyboardNext.SetActive(false);
        //     }
        //     else {
        //         interactGamepadPrevious.SetActive(false);
        //         interactGamepadNext.SetActive(false);
        //         interactKeyboardPrevious.SetActive(true);
        //         interactKeyboardNext.SetActive(true);
        //     }
        // }
        
        if (_controllerHelper != null) {
            if (_controllerHelper.isSwitchController || _controllerHelper.isXboxController ||
                _controllerHelper.isPSController) {
                interactGamepadPreviousNew.SetActive(true);
                interactGamepadNextNew.SetActive(true);
                interactKeyboardPreviousNew.SetActive(false);
                interactKeyboardNextNew.SetActive(false);
            }
            else {
                interactGamepadPreviousNew.SetActive(false);
                interactGamepadNextNew.SetActive(false);
                interactKeyboardPreviousNew.SetActive(true);
                interactKeyboardNextNew.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Close the journal
    /// </summary>
    public void CloseJournal() {
        //pages[_activePageIndex].GetComponent<JournalPage>().CloseTab();
        pagesNews[_activePageIndex].GetComponent<JournalPage>().CloseTab();
        
        _desiredAlpha = 0;
        _isJournalOpen = false;
        
        //canvasGroup.interactable = false;
        //canvasGroup.blocksRaycasts = false;
        canvasGroupNew.interactable = false;
        canvasGroupNew.blocksRaycasts = false;
        
        PostProcessingManager.Instance.TurnOffJournal();

        UserInputs.Instance.OnCloseJournal();
        
        cinemachineCamera.SetActive(false);


    }

    /// <summary>
    /// Sets the journal to the first page
    /// </summary>
    private void GoToFirstPage() {
        // foreach (var page in pages) {
        //     page.gameObject.SetActive(false);
        // }
        
        foreach (var page in pagesNews) {
            page.gameObject.SetActive(false);
        }

        pagesNews[0].gameObject.SetActive(true);
        _activePageIndex = 0;
        SetActivePage(_activePageIndex);
    }


    /// <summary>
    /// Sets the journal to the sent in index page
    /// </summary>
    public void OpenJournalToPage(int index) {
        _activePageIndex = index;
        // for (int i = 0; i < pages.Length; i++) {
        //     pages[i].gameObject.SetActive(false);
        //     if (_activePageIndex == i) {
        //         pages[i].gameObject.SetActive(true);
        //         SetActivePage(_activePageIndex);
        //     }
        // }
        
        for (int i = 0; i < pagesNews.Length; i++) {
            pagesNews[i].gameObject.SetActive(false);
            if (_activePageIndex == i) {
                pagesNews[i].gameObject.SetActive(true);
                SetActivePage(_activePageIndex);
            }
        }

        _desiredAlpha = 1;
        _isJournalOpen = true;
        
        // canvasGroup.interactable = true;
        // canvasGroup.blocksRaycasts = true;
        canvasGroupNew.interactable = true;
        canvasGroupNew.blocksRaycasts = true;
        ShowCorrectButton();
        PostProcessingManager.Instance.TurnOnJournal();

        UserInputs.Instance.OnOpenJournal();
        
        cinemachineCamera.SetActive(true);
        UIPromptCanvas.Instance.HidePrompt();
    }

    /// <summary>
    /// Go to next page in journal
    /// </summary>
    private void NextPage() {
        //pages[_activePageIndex].GetComponent<JournalPage>().CloseTab();
        pagesNews[_activePageIndex].GetComponent<JournalPage>().CloseTab();
        _activePageIndex++;

        // if (_activePageIndex > pages.Length - 1)
        //     _activePageIndex = 0;

        if (_activePageIndex > pagesNews.Length - 1)
            _activePageIndex = 0;
        
        // for (int i = 0; i < pages.Length; i++) {
        //     pages[i].gameObject.SetActive(false);
        //     if (_activePageIndex == i) {
        //         pages[i].gameObject.SetActive(true);
        //         SetActivePage(_activePageIndex);
        //     }
        // }
        
        for (int i = 0; i < pagesNews.Length; i++) {
            pagesNews[i].gameObject.SetActive(false);
            if (_activePageIndex == i) {
                pagesNews[i].gameObject.SetActive(true);
                SetActivePage(_activePageIndex);
            }
        }
    }

    /// <summary>
    /// Go to previous page in journal
    /// </summary>
    private void PreviousPage() {
        //pages[_activePageIndex].GetComponent<JournalPage>().CloseTab();
        //pages[_activePageIndex].gameObject.SetActive(false);
        pagesNews[_activePageIndex].GetComponent<JournalPage>().CloseTab();
        pagesNews[_activePageIndex].gameObject.SetActive(false);
        
        _activeJournalPage.CloseTab();

        _activePageIndex--;

        // if (_activePageIndex < 0)
        //     _activePageIndex = pages.Length - 1;
        
        if (_activePageIndex < 0)
            _activePageIndex = pagesNews.Length - 1;

        //pages[_activePageIndex].gameObject.SetActive(true);
        pagesNews[_activePageIndex].gameObject.SetActive(true);
        SetActivePage(_activePageIndex);
    }

    /// <summary>
    /// Set the page to the active page from index
    /// </summary>
    /// <param name="pageIndex"></param>
    private void SetActivePage(int pageIndex) {
        //_activeJournalPage = pages[pageIndex].GetComponent<JournalPage>();
        _activeJournalPage = pagesNews[pageIndex].GetComponent<JournalPage>();
        _activeJournalPage.OpenTab();
    }

    /// <summary>
    /// Input to open menu
    /// </summary>
    /// <param name="obj"></param>
    private void OpenMenuInput(InputAction.CallbackContext obj) {
        ToggleJournal();
    }

    /// <summary>
    /// Input to go to next page
    /// </summary>
    /// <param name="obj"></param>
    private void NextInput(InputAction.CallbackContext obj) {
        if (_isJournalOpen)
            NextPage();
    }

    /// <summary>
    /// Input to go to previous page
    /// </summary>
    /// <param name="obj"></param>
    private void PreviousInput(InputAction.CallbackContext obj) {
        if (_isJournalOpen)
            PreviousPage();
    }
}