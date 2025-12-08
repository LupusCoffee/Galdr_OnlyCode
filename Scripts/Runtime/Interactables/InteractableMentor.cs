using System.Collections;
using UnityEngine;
using TMPro;
using static CompactMath;

public class InteractableMentor : Interactable
{
    
    [SerializeField] GameObject dialogueBoxPopup;

    [SerializeField] DialogueLockPair[] dialogues;

    float talkCooldown = 5;
    float currentCooldown = 0;

    void Update() {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
        }
    }

    public override SO_InteractableData Interact() {
        if (currentCooldown > 0) {
            return null;
        }

        currentCooldown = talkCooldown;

        GameObject gb = Instantiate(dialogueBoxPopup,
            transform.position + new Vector3(-0.75f, 2, 0), Quaternion.identity);

        gb.transform.localScale = Vector3.zero;

        //rotate away from player
        gb.transform.LookAt(FirstPersonController.Instance.transform);
        gb.transform.Rotate(0, 180, 0);
        gb.transform.rotation = Quaternion.Euler(0, gb.transform.rotation.eulerAngles.y, 0);

        gb.GetComponent<RotateTextTowardPlayer>().SetPivot(transform.position);


        LeanTween.scale(gb, Vector3.one, 0.25f);
        StartCoroutine(TextDecay(gb));

        var result = dialogues[Random.Range(0, dialogues.Length)].GetDialogue();

        TextMeshPro textComponent = gb.GetComponentInChildren<TextMeshPro>();
        textComponent.text = result.text;

        if (result.encrypted)
        {
            textComponent.UpdateFontAsset();
        }


        //Debug.Log("Interacting with Mentor");
        //JournalManager.Instance.OpenJournalToPage(1);

        return this.data;
    }

    private IEnumerator TextDecay(GameObject target)
    {
        yield return new WaitForSeconds(3);
        LeanTween.scale(target, Vector3.zero, 0.1f).setOnComplete(()
            => Destroy(target));
    }

    public override bool ChangeState(InteractionState interactionState)
    {
        this.state = interactionState;
        return true;
    }
}
