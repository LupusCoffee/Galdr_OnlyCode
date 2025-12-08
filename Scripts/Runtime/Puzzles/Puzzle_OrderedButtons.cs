using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static CompactMath;

public class Puzzle_OrderedButtons : PuzzleEvent
{
    [SerializeField] List<AK.Wwise.Event> soundOrder;
    [SerializeField] List<GameObject> orderedButtons; //this is the order that needs to be followed
    [SerializeField] Puzzle_SpellButton startButton;
    List<Puzzle_ActivateButton> activateButtons = new List<Puzzle_ActivateButton>();


    private List<GameObject> currentOrder = new List<GameObject>();

    ParticleSystem ParticleSystem;

    void Awake()
    {
        ParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        //listen to all buttons
        foreach (GameObject button in orderedButtons)
        {
            button.GetComponent<Puzzle_ActivateButton>().OnButtonPressed += ButtonPressed;
            activateButtons.Add(button.GetComponent<Puzzle_ActivateButton>());
        }

        // why is this throwing errors
        startButton.OnButtonPressed += StartButtonPressed;
    }

    private void ButtonPressed(GameObject obj)
    {
        //add the button to the current order
        currentOrder.Add(obj);
        //check if the current order is the same as the code

        MusicManager.Instance.PostEvent(soundOrder[activateButtons.IndexOf(obj.GetComponent<Puzzle_ActivateButton>())]);

        //check if the current index matches the ordered index

        if (currentOrder[currentOrder.Count - 1] != orderedButtons[currentOrder.Count - 1]) {
            StartCoroutine(WrongOrder());
            ResetButtons();

            Player.Instance.DisableOrderedButtonInteraction();

            return;
        } else if (currentOrder.Count == orderedButtons.Count) {
            Debug.Log("Correct order!");
            TriggerOnButtonPressed();

            Player.Instance.DisableOrderedButtonInteraction();
        } else {
            Player.Instance.ActivatedOneOrderedButton();
        }
    }

    private async void ResetButtons()
    {
        await Task.Delay(SecondsToMilli(0.5F));

        foreach (Puzzle_ActivateButton button in activateButtons)
        {
            button.StartCoroutine("ResetButton");
        }

        foreach (Puzzle_ActivateButton button in activateButtons)
        {
            button.isPressable = false;
        }

        ParticleSystem.Stop();
    }

    public IEnumerator WrongOrder()
    {
        yield return new WaitForSeconds(1);

        MusicManager.Instance.PostEvent("Puzzle_OrderedButtonsWrong");

        yield return null;
    }

    public IEnumerator StartPuzzle()
    {
        currentOrder.Clear();

        yield return new WaitForSeconds(2);

        MusicManager.Instance.PostEvent("Puzzle_OrderedButtonsCorrectMelody");

        foreach (Puzzle_ActivateButton button in activateButtons)
        {
            button.ShowOrder();
            yield return new WaitForSeconds(0.75f);
        }

        yield return new WaitForSeconds(1);

        foreach (Puzzle_ActivateButton button in activateButtons)
            button.isPressable = true;

        ParticleSystem.Play();

        Player.Instance.EnableOrderedButtonInteraction();

        yield return null;
    }

    private void StartButtonPressed(GameObject obj)
    {
        StartCoroutine(StartPuzzle());
    }

}
