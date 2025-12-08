using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using GameObject = UnityEngine.GameObject;
using Image = UnityEngine.UI.Image;

public enum FaceButtonsState
{
    SpellEffectAndInput,
    InputOnly
}

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    [Header("Controller Icons")] //todo: put this somewhere else
    [SerializeField] GamepadIcons xboxIcons;
    [SerializeField] GamepadIcons playstationIcons;
    [SerializeField] GamepadIcons switchIcons;
    [SerializeField] KeyboardIcons keyboardIcons;
    
    [Header("InputSpellController - set from children")]
    [SerializeField] 
    private CanvasGroup inputSpellControllerCanvasGroup;

    [SerializeField]
    private GameObject buttonsForSpellEffectsAndInputIconsParent;
    
    [SerializeField]
    private GameObject buttonsForInputIconsOnlyParent;
    
    [SerializeField]
    private GameObject spellEffectIconsParent;
    
    [SerializeField]
    private HorizontalLayoutGroup layoutGroupRegularBars; 
    
    [SerializeField]
    private HorizontalLayoutGroup layoutGroupShadowBars;
    
    [SerializeField]
    private Image activeSpellEffectImage;
    
    [Header("InputSpellController - set from prefabs, data, etc.")]
    [SerializeField]
    private GameObject inputSpellBarPrefab;
    
    [SerializeField]
    private Sprite emptySpellEffectIcon; //readonly + SerializeField how?
    
    private Dictionary<InputSpellInput, Image> faceButtonSpellEffectImages = new();

    private List<UIInputSpellBar> currentRegularInputSpellBars = new();
    private List<UIInputSpellBar> currentShadowInputSpellBars = new();

    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        if (spellEffectIconsParent == null) return;

        int i = 0;
        foreach (Transform spellEffectIcon in spellEffectIconsParent.transform)
        {
            if(faceButtonSpellEffectImages.TryAdd((InputSpellInput)i, spellEffectIcon.GetComponent<Image>()))
            {
                i++;
            }
        }
    }
    
    
    //Face Buttons & Spell Effects
    public void ToggleFaceButtonIcons(FaceButtonsState state)
    {
        if(!buttonsForSpellEffectsAndInputIconsParent) return;
        if(!buttonsForInputIconsOnlyParent) return;
        
        if (state == FaceButtonsState.SpellEffectAndInput)
        {
            buttonsForSpellEffectsAndInputIconsParent.SetActive(true);
            buttonsForInputIconsOnlyParent.SetActive(false);
        }
        else
        {
            buttonsForSpellEffectsAndInputIconsParent.SetActive(false);
            buttonsForInputIconsOnlyParent.SetActive(true);
        }
    }
    public void UpdateAllSpellEffectFaceButtonIconsAvailability(List<SpellEffectRunTimeData> spellEffectRunTimeDatas)
    { 
        foreach (var spellEffectRunTimeData in spellEffectRunTimeDatas)
            UpdateSpellEffectFaceButtonIconAvailability(spellEffectRunTimeData);
    }
    public void UpdateSpellEffectFaceButtonIconAvailability(SpellEffectRunTimeData spellEffectRunTimeData)
    {
        Sprite spellEffectIcon = GetSpellEffectIconConsideringAvailability(spellEffectRunTimeData);

        if (!faceButtonSpellEffectImages.TryGetValue(spellEffectRunTimeData.GetInputSpellInput(),
                out Image faceButtonIcon)) return;
        
        faceButtonIcon.sprite = spellEffectIcon;
    }
    
    public void SetActiveSpellEffectIcon(InputSpellInput inputSpellInput)
    {
        activeSpellEffectImage.sprite = GetSpecificSpellEffectIcon(inputSpellInput, true);
    }
    public void DisplayActiveSpellEffect(bool value)
    {
        activeSpellEffectImage.gameObject.SetActive(value);
    }
    
    private Sprite GetSpellEffectIconConsideringAvailability(SpellEffectRunTimeData spellEffectRunTimeData)
    {
        Sprite spellEffectIcon = null;
        
        bool isAvailable = spellEffectRunTimeData.IsAvailable();
        if (spellEffectRunTimeData.TryGetSpellEffectSO(out var spellEffectSO))
            spellEffectIcon = spellEffectSO.GetSpellEffectIcon(isAvailable);
            
        if (!spellEffectIcon) spellEffectIcon = emptySpellEffectIcon;
        return spellEffectIcon;
    }
    private Sprite GetSpecificSpellEffectIcon(InputSpellInput inputSpellInput, bool isAvailable) //very similar to above. bothers me that it uses input hmmm
    {
        Sprite spellEffectIcon = null;

        if (SpellManager.Instance.TryGetUnlockedSpellEffectRunTimeData(inputSpellInput, out var spellEffectRunTimeData))
        {
            if (spellEffectRunTimeData.TryGetSpellEffectSO(out var spellEffectSO))
                spellEffectIcon = spellEffectSO.GetSpellEffectIcon(isAvailable);
        }
        
        if (!spellEffectIcon) spellEffectIcon = emptySpellEffectIcon;
        return spellEffectIcon;
    }
    
    
    //Shadow and Regular Bars
    public void PopulateBarLayoutGroups(List<InputSpellNote> inNoteSequence)
    {
        //Why do it like this? It limits the amount of times we instantiate, destroy, and call GetComponent.
        //Best case, we only instantiate once.
        
        int elementCountDelta = inNoteSequence.Count - currentRegularInputSpellBars.Count; //it's fine to check only regular, since shadow will always be the same size
        if (elementCountDelta > 0)
        {
            AddInputSpellBars(elementCountDelta, currentRegularInputSpellBars, layoutGroupRegularBars);
            AddInputSpellBars(elementCountDelta, currentShadowInputSpellBars, layoutGroupShadowBars);
        }
        else if (elementCountDelta < 0)
        {
            int absoluteElementCountDelta = Mathf.Abs(elementCountDelta);
            RemoveInputSpellBars(absoluteElementCountDelta, currentRegularInputSpellBars);
            RemoveInputSpellBars(absoluteElementCountDelta, currentShadowInputSpellBars);
        }
        
        GenerateRelevantShadowBarsLayoutGroup(inNoteSequence);
        DisableAllNotesInRegularBarLayoutGroup();
    }
    private void AddInputSpellBars(int amount, List<UIInputSpellBar> inputSpellBars, LayoutGroup parent)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject inputSpellBarObj = Instantiate(inputSpellBarPrefab, Vector3.zero, Quaternion.identity, parent.transform);
            UIInputSpellBar inputSpellBar = inputSpellBarObj.GetComponent<UIInputSpellBar>();
            inputSpellBars.Add(inputSpellBar);
        }
    }
    private void RemoveInputSpellBars(int amount, List<UIInputSpellBar> inputSpellBars)
    {
        for (int i = 0; i < amount; i++)
        {
            int lastIndex = inputSpellBars.Count - 1;
            if (inputSpellBars[lastIndex] == null)
            {
                Debug.LogWarning("Bar at " + lastIndex + " in inputSpellBars is null, " + 
                                 "but you are trying to remove it.");
                continue;
            }
            
            UIInputSpellBar barToRemove = inputSpellBars[lastIndex];
            inputSpellBars.Remove(barToRemove);
            Destroy(barToRemove.gameObject);
        }
    }
    private void GenerateRelevantShadowBarsLayoutGroup(List<InputSpellNote> inNoteSequence)
    {
        for (int i = 0; i < inNoteSequence.Count; i++)
        {
            InputSpellNote note = inNoteSequence[i];
            currentShadowInputSpellBars[i].SetNoteOnBar(note);
        }
    }
    
    public void DisableAllNotesInRegularBarLayoutGroup()
    {
        foreach (var inputSpellBar in currentRegularInputSpellBars)
            inputSpellBar.DisableAllNotes();
    }
    public void ShowNoteInRegularBarLayoutGroup(int barNum, InputSpellNote note)
    {
        currentRegularInputSpellBars[barNum].SetNoteOnBar(note);
    }
    
    public void DisplayBarLayoutGroups(bool enable)
    {
        layoutGroupRegularBars.gameObject.SetActive(enable);
        layoutGroupShadowBars.gameObject.SetActive(enable);
    }
    

    //Getters
    public CanvasGroup GetInputSpellControllerCanvas() => inputSpellControllerCanvasGroup;
    public GamepadIcons GetGamepadIcons() => playstationIcons;
    public KeyboardIcons GetKeyboardIcons() => keyboardIcons;

    //Game Icons
    [Serializable]
    public struct GamepadIcons
    {
        public Sprite buttonSouth;
        public Sprite buttonNorth;
        public Sprite buttonEast;
        public Sprite buttonWest;
        public Sprite startButton;
        public Sprite selectButton;
        public Sprite leftTrigger;
        public Sprite rightTrigger;
        public Sprite leftShoulder;
        public Sprite rightShoulder;
        public Sprite dpad;
        public Sprite dpadUp;
        public Sprite dpadDown;
        public Sprite dpadLeft;
        public Sprite dpadRight;
        public Sprite leftStick;
        public Sprite rightStick;
        public Sprite leftStickPress;
        public Sprite rightStickPress;

        public Sprite GetSprite(string controlPath)
        {
            // From the input system, we get the path of the control on device. So we can just
            // map from that to the sprites we have for gamepads.
            switch (controlPath)
            {
                case "buttonSouth": return buttonSouth;
                case "buttonNorth": return buttonNorth;
                case "buttonEast": return buttonEast;
                case "buttonWest": return buttonWest;
                case "start": return startButton;
                case "select": return selectButton;
                case "leftTrigger": return leftTrigger;
                case "rightTrigger": return rightTrigger;
                case "leftShoulder": return leftShoulder;
                case "rightShoulder": return rightShoulder;
                case "dpad": return dpad;
                case "dpad/up": return dpadUp;
                case "dpad/down": return dpadDown;
                case "dpad/left": return dpadLeft;
                case "dpad/right": return dpadRight;
                case "leftStick": return leftStick;
                case "rightStick": return rightStick;
                case "leftStickPress": return leftStickPress;
                case "rightStickPress": return rightStickPress;
            }

            return null;
        }
    }

    [Serializable]
    public struct KeyboardIcons
    {
        public Sprite leftMouseButton;
        public Sprite rightMouseButton;
        public Sprite middleMouseButton;
        public Sprite mouseScroll;
        public Sprite mouseButtonForward;
        public Sprite mouseButtonBack;
        public Sprite one;
        public Sprite two;
        public Sprite three;
        public Sprite four;
        public Sprite five;
        public Sprite six;
        public Sprite seven;
        public Sprite eight;
        public Sprite nine;
        public Sprite zero;
        public Sprite A;
        public Sprite B;
        public Sprite C;
        public Sprite D;
        public Sprite E;
        public Sprite F;
        public Sprite G;
        public Sprite H;
        public Sprite I;
        public Sprite J;
        public Sprite K;
        public Sprite L;
        public Sprite M;
        public Sprite N;
        public Sprite O;
        public Sprite P;
        public Sprite Q;
        public Sprite R;
        public Sprite S;
        public Sprite T;
        public Sprite U;
        public Sprite V;
        public Sprite W;
        public Sprite X;
        public Sprite Y;
        public Sprite Z;
        public Sprite Enter;
        public Sprite TAB;
        public Sprite CAPS;
        public Sprite shift;
        public Sprite Alt;
        public Sprite backSpace;
        public Sprite num1;
        public Sprite num2;
        public Sprite num3;
        public Sprite num4;
        public Sprite num5;
        public Sprite num6;
        public Sprite num7;
        public Sprite num8;
        public Sprite num9;
        public Sprite num0;
        public Sprite F1;
        public Sprite F2;
        public Sprite F3;
        public Sprite F4;
        public Sprite F5;
        public Sprite F6;
        public Sprite F7;
        public Sprite F8;
        public Sprite F9;
        public Sprite F10;
        public Sprite F11;
        public Sprite F12;
        public Sprite HOME;
        public Sprite END;
        public Sprite INS;
        public Sprite DEL;
        public Sprite PGUP;
        public Sprite PGDOWN;
        public Sprite arrowDown;
        public Sprite arrowUp;
        public Sprite arrowRight;
        public Sprite arrowLeft;
        public Sprite comma;
        public Sprite ctrl;
        public Sprite escape;
        public Sprite numLock;
        public Sprite numPadEnter;
        public Sprite numPadPlus;
        public Sprite numPadMinus;
        public Sprite numPadDivide;
        public Sprite numPadMultiply;
        public Sprite period;
        public Sprite printScreen;
        public Sprite quote;
        public Sprite space;
        public Sprite tilde;
        public Sprite mouseScrollDown;
        public Sprite mouseScrollUp;
        public Sprite mouseDelta;

        public Sprite GetSprite(string controlPath)
        {
            switch (controlPath)
            {
                case "leftButton": return leftMouseButton;
                case "rightButton": return rightMouseButton;
                case "middleButton": return middleMouseButton;
                case "scroll": return mouseScroll;
                case "forwardButton": return mouseButtonForward;
                case "backButton": return mouseButtonBack;
                case "1": return one;
                case "2": return two;
                case "3": return three;
                case "4": return four;
                case "5": return five;
                case "6": return six;
                case "7": return seven;
                case "8": return eight;
                case "9": return nine;
                case "0": return zero;
                case "a": return A;
                case "b": return B;
                case "c": return C;
                case "d": return D;
                case "e": return E;
                case "f": return F;
                case "g": return G;
                case "h": return H;
                case "i": return I;
                case "j": return J;
                case "k": return K;
                case "l": return L;
                case "m": return M;
                case "n": return N;
                case "o": return O;
                case "p": return P;
                case "q": return Q;
                case "r": return R;
                case "s": return S;
                case "t": return T;
                case "u": return U;
                case "v": return V;
                case "w": return W;
                case "x": return X;
                case "y": return Y;
                case "z": return Z;
                case "enter": return Enter;
                case "tab": return TAB;
                case "capsLock": return CAPS;
                case "shift": return shift;
                case "alt": return Alt;
                case "backspace": return backSpace;
                case "numpad1": return num1;
                case "numpad2": return num2;
                case "numpad3": return num3;
                case "numpad4": return num4;
                case "numpad5": return num5;
                case "numpad6": return num6;
                case "numpad7": return num7;
                case "numpad8": return num8;
                case "numpad9": return num9;
                case "numpad0": return num0;
                case "f1": return F1;
                case "f2": return F2;
                case "f3": return F3;
                case "f4": return F4;
                case "f5": return F5;
                case "f6": return F6;
                case "f7": return F7;
                case "f8": return F8;
                case "f9": return F9;
                case "f10": return F10;
                case "f11": return F11;
                case "f12": return F12;
                case "home": return HOME;
                case "end": return END;
                case "insert": return INS;
                case "delete": return DEL;
                case "pageUp": return PGUP;
                case "pageDown": return PGDOWN;
                case "downArrow": return arrowDown;
                case "upArrow": return arrowUp;
                case "rightArrow": return arrowRight;
                case "leftArrow": return arrowLeft;
                case "comma": return comma;
                case "ctrl": return ctrl;
                case "escape": return escape;
                case "numLock": return numLock;
                case "numpadEnter": return numPadEnter;
                case "numpadPlus": return numPadPlus;
                case "numpadMinus": return numPadMinus;
                case "numpadDivide": return numPadDivide;
                case "numpadMultiply": return numPadMultiply;
                case "period": return period;
                case "printScreen": return printScreen;
                case "quote": return quote;
                case "space": return space;
                case "backquote": return tilde;
                case "scroll/down": return mouseScrollDown;
                case "scroll/up": return mouseScrollUp;
                case "delta": return mouseDelta;
            }

            return null;
        }
    }

    [Serializable]
    public struct MouseIcons
    {
        public Sprite leftMouseButton;
        public Sprite rightMouseButton;
        public Sprite middleMouseButton;
        public Sprite mouseScroll;
        public Sprite mouseButtonForward;
        public Sprite mouseButtonBack;
        public Sprite mouseScrollDown;
        public Sprite mouseScrollUp;
        public Sprite mouseDelta;

        public Sprite GetSprite(string controlPath)
        {
            switch (controlPath)
            {
                case "leftButton": return leftMouseButton;
                case "rightButton": return rightMouseButton;
                case "middleButton": return middleMouseButton;
                case "scroll": return mouseScroll;
                case "forwardButton": return mouseButtonForward;
                case "backButton": return mouseButtonBack;
                case "scroll/down": return mouseScrollDown;
                case "scroll/up": return mouseScrollUp;
                case "delta": return mouseDelta;
            }

            return null;
        }
    }
}
