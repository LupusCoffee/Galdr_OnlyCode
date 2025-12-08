using TMPro;
using UnityEngine;

public class TutorialPrompts : MonoBehaviour
{
    [SerializeField] TutorialEnum tutorialEnum;
    [SerializeField] string header;
    [SerializeField] string message;

    private void OnTriggerEnter(Collider other)
    {
        //other.gameObject.TryGetComponent(out Player player);
        if (other.gameObject.CompareTag("Player")) {

            if (tutorialEnum == TutorialEnum.move)
            {
                UIPromptCanvas.Instance.ShowMovePrompt();
            }
            if (tutorialEnum == TutorialEnum.look)
            {
                UIPromptCanvas.Instance.ShowLookPrompt();
            }
            if (tutorialEnum == TutorialEnum.message)
            {
                UIPromptCanvas.Instance.ShowMessagePrompt(header, message);
            }
        }

    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            UIPromptCanvas.Instance.HidePrompt();
        }
    }

}

public enum TutorialEnum
{
    move,
    look,
    message,
}

