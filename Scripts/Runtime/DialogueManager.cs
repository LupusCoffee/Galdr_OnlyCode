using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialogueBackground;
    [SerializeField] GameObject dialogueForeground;

    [SerializeField] TMPro.TextMeshProUGUI dialogueText;

    void Start()
    {
        Debug.Log("Hello World!");
    }

}
