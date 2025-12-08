using System;
using UnityEngine;
using UnityEngine.UI;

public class Song : MonoBehaviour {

    //[SerializeField] SO_Song songSO; //TO-DO: the whole canvas will have to refernece the song manager
    public GameObject highlightObject;
    private Button _button;

    private void Awake() {
        _button = GetComponent<Button>();
    }

    public void Interact() 
    {
        string notes = "";

        /*foreach (var note in songSO.GetFullSequence())
            notes += Enum.GetName(note.GetType(), note) + " ";*/

        //SongManager.Instance.SetSong(notes);
    }

    public void Highlight() {
        highlightObject.SetActive(true);
        _button.Select();
    }

    public void RemoveHighlight() {
        highlightObject.SetActive(false);
    }

    public Button GetButton() {
        return _button;
    }
}
