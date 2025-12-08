using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    public int LanguageLevel = 0;

    public static FirstPersonController Instance { get; private set; }


    private bool canMove = true;

    public float speed = 5;
    Rigidbody rb;
    [SerializeField] GameObject playerCamera;

    PlayerInteractZone interactZone;


    void Awake()
    {
        Instance = this;


        rb = GetComponent<Rigidbody>();
        playerCamera.transform.position = transform.position + Vector3.up * 0.5f;

        interactZone = GetComponentInChildren<PlayerInteractZone>();
    }

    public void OnEnable()
    {
        UserInputs.Instance._playerInteract.performed += OnInteract;
    }

    public void OnDisable()
    {
        UserInputs.Instance._playerInteract.performed -= OnInteract;
    }

    void FixedUpdate()
    {
        if (!canMove)
            return;

        Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal") * speed, Input.GetAxis("Vertical") * speed);
        rb.linearVelocity = transform.rotation * new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.y);
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
    }

    public void OnInteract(InputAction.CallbackContext obj)
    {
        Interactable interactable = interactZone.GetLastInteractable();
        if (interactable != null)
        {
            /*if (interactable.GetType() == typeof(Puzzle_ActivateButton)) {
                if (((Puzzle_ActivateButton)interactable).IsButtonActive())
                    Debug.Log("Puzzle Button can be interacted with");
                else
                    Debug.Log("Puzzle Button can not be interacted with");
            }*/

            SO_InteractableData result = interactable.Interact();

            if (result != null)
            {
                switch (interactable)
                {
                    case InteractableArtifact artifact:
                        //Debug.Log(artifact.ToString());
                        Inventory.TryCollectItem(artifact); // Collect the item - Added by Martin M
                        artifact.GetGameObject().GetComponent<Collider>().enabled = false;
                        //artifact.GetGameObject().GetComponent<MeshRenderer>().enabled = false;
                        JournalManager.Instance.OpenJournalToPage(0);
                        break;
                    case InteractableSignPost signPost:
                        //Debug.Log(signPost.ToString());
                        break;
                    case InteractableNPC npc:
                        //Debug.Log(npc.ToString());
                        Inventory.TryUnlockNpc(npc); // Unlock the NPC - Added by Martin M
                        break;
                    case InteractableTransition transition:
                        //Debug.Log(transition.ToString());
                        break;
                    case InteractableDoor door:
                        //Debug.Log(door.ToString());
                        break;
                    case InteractableSpellAttribute spellAttribute:
                        //Debug.Log(spellAttribute.ToString());
                        Inventory.TryCollectItem(spellAttribute); // Collect the item - Added by Martin M
                        spellAttribute.GetGameObject().GetComponent<Collider>().enabled = false;
                        JournalManager.Instance.OpenJournalToPage(2);
                        break;
                    default: // If nothing matches
                        //Debug.Log(interactable.GetType().Name);
                        break;
                }
            }
            else
            {
                if (interactable.GetType() == typeof(Puzzle_ActivateButton))
                {
                    /*if (((Puzzle_ActivateButton)interactable).HasBeenPressed())
                        Debug.Log("Puzzle Button has been pressed");
                    else
                        Debug.Log("Puzzle Button has not been pressed");*/
                }
            }
        }
        else
        {
            //Debug.Log("No interactable found");
        }
    }
}
