using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static MultiTag;

public class Grabber : Enemy {
    MultiTags[] grabbableTags = { MultiTags.Player, MultiTags.Possess_Grabbable, MultiTags.Pickups };

    Possessable possessable;
    private bool active = false;

    private List<GameObject> grabbableObjectsInRange = new List<GameObject>();
    GameObject grabbedObject;

    Vector3 grabbedObjectOriginalScale = Vector3.one;
    Rigidbody grabbedObjRB;

    float throwCooldown = 1f;
    float throwTimer = 0;

    [SerializeField] private Animator animator;

    float grabCooldown = 3;
    float currentGrabCooldown = 0;

    private void Awake() {
        this.possessable = this.GetComponent<Possessable>();
    }

    public override void OnEnable() {
        base.OnEnable();
        UserInputs.Instance._playerInteract.performed += OnInteract;
    }

    public override void OnDisable() {
        base.OnDisable();
        UserInputs.Instance._playerInteract.performed -= OnInteract;
    }

    protected override void Start() {
        base.Start();
    }

    private void Update()
    {
        if (this.throwTimer > 0)
        {
            this.throwTimer -= Time.deltaTime;
        }

        /*rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (mindControl.isMindControlled)
        {
            ReleasePlayer();
        }*/

        if (this.possessable.hasBeenPossessed && this.grabbableObjectsInRange.Count > 0 &&
                this.possessable.isMindControlled) {
            if (this.GetLastGrabbableObject() == Player.Instance.gameObject) {
                this.ReleasePlayer();
                //Player.Instance.SetGrabbed(false);
            }

            InteractPrompt.Instance.ShowInteractPrompt(InteractPrompt.Instance.devourerSprite);
            this.GetLastGrabbableObject().GetComponent<OutlineObject>().SetOutlinePink(true);

            this.possessable.hasBeenPossessed = false;
        } else if (this.possessable.hasBeenPossessed && this.grabbableObjectsInRange.Count == 0) {
            this.possessable.hasBeenPossessed = false;
        }

        if (this.possessable.hasBeenUnpossessed && this.grabbableObjectsInRange.Count > 0 &&
                !this.possessable.isMindControlled) {
            //check if the player has anything in InteractZone?
            InteractPrompt.Instance.HideInteractPrompt();
            GetLastGrabbableObject().GetComponent<OutlineObject>().SetOutlinePink(false);

            this.possessable.hasBeenUnpossessed = false;
        } else if (this.possessable.hasBeenUnpossessed && this.grabbableObjectsInRange.Count == 0) {
            this.possessable.hasBeenUnpossessed = false;
        }

    }

    private void OnTriggerEnter(Collider other) {
        if (this.possessable.isMindControlled) {
            if (HasTag(other.gameObject, this.grabbableTags)) {
                this.grabbableObjectsInRange.Add(other.gameObject);

                if (this.grabbedObject == null) {
                    if (GetLastGrabbableObject().TryGetComponent(out OutlineObject newOutlineObject)) {
                        newOutlineObject.SetOutlinePink(true);
                        InteractPrompt.Instance.ShowInteractPrompt(InteractPrompt.Instance.devourerSprite);
                    }

                    if (this.grabbableObjectsInRange.Count > 1 &&
                            this.grabbableObjectsInRange[^2].
                            TryGetComponent(out OutlineObject oldOutlineObject)) {
                        oldOutlineObject.SetOutlinePink(false);
                    }
                }
            } 
        } else {
            this.currentGrabCooldown -= Time.deltaTime;
            if (this.currentGrabCooldown > 0) { return; }
            if (HasTag(other.gameObject, this.grabbableTags)) { 
                this.grabbableObjectsInRange.Add(other.gameObject);
                if (other.gameObject == Player.Instance.gameObject) {
                    this.GrabPlayer();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (HasTag(other.gameObject, this.grabbableTags)) {
            this.grabbableObjectsInRange.Remove(other.gameObject);

            if (this.possessable.isMindControlled) {
                if (HasTag(other.gameObject, this.grabbableTags)) {
                    this.grabbableObjectsInRange.Remove(other.gameObject);

                    if (other.gameObject.TryGetComponent(out OutlineObject oldOutlineObject)) {
                        oldOutlineObject.SetOutlinePink(false);
                    }

                    if(this.grabbedObject == null) {
                        if (this.grabbableObjectsInRange.Count > 0 &&
                                GetLastGrabbableObject().TryGetComponent(out OutlineObject nextOutlineObject)) {
                            nextOutlineObject.SetOutlinePink(true);
                        } else if (this.grabbableObjectsInRange.Count == 0) {
                            InteractPrompt.Instance.HideInteractPrompt();
                        }
                    } 
                }
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext ctx) {
        if (!this.possessable.isMindControlled) { return; }


        if (this.grabbedObject == null) {
            SfxManager.Instance.PostEvent("Play_DevourerGrab", gameObject);
            if (this.GetLastGrabbableObject() != null) { this.PlayerControl_Activate(); }
        } else {
            SfxManager.Instance.PostEvent("Play_DevourerLetGo", gameObject);
            this.PlayerControl_Deactivate();
        }
    }

    public GameObject GetFirstGrabbableObject() {
        if (this.grabbableObjectsInRange.Count > 0)
            return this.grabbableObjectsInRange[0];
        return null;
    }

    public GameObject GetLastGrabbableObject() {
        if (this.grabbableObjectsInRange.Count > 0)
            return this.grabbableObjectsInRange[^1];
        return null;
    }

    public override void PlayerControl_Activate() {
        if (this.throwTimer > 0) return;

        if (this.grabbableObjectsInRange.Count > 0) {
            this.grabbedObject = GetLastGrabbableObject();
            this.grabbableObjectsInRange.Remove(this.grabbedObject);

            InteractPrompt.Instance.ShowInteractPrompt(InteractPrompt.Instance.throwSprite);

            if (this.grabbedObject.TryGetComponent(out OutlineObject grabbedObject)) {
                grabbedObject.SetOutlinePink(false);
            }

            this.active = true;
            this.grabbedObjRB = this.grabbedObject.GetComponent<Rigidbody>();
            this.grabbedObjRB.isKinematic = true;
            this.grabbedObjectOriginalScale = this.grabbedObject.transform.localScale;

            this.possessable.SetCanMove(false);

            LeanTween.scale(this.grabbedObject, Vector3.zero, 0.25f);
            LeanTween.move(this.grabbedObject, transform.position + Vector3.down, 0.5f).setOnComplete(() => {
                this.possessable.SetCanMove(true);
            });

            // Trigger the "Grab" animation
            animator.SetTrigger("Grab");

            this.throwTimer = this.throwCooldown;
        }
    }

    public override void PlayerControl_Deactivate() {
        if (this.throwTimer > 0) return;

        this.active = false;
        if (this.grabbedObject == null) return;

        //change order of this code?
        if (this.grabbableObjectsInRange.Count == 0) {
            InteractPrompt.Instance.HideInteractPrompt();
        } else {
            InteractPrompt.Instance.ShowInteractPrompt(InteractPrompt.Instance.devourerSprite);
        }

        this.possessable.SetCanMove(false);
        this.grabbedObject.transform.position = transform.position + Vector3.down;

        LeanTween.scale(this.grabbedObject, this.grabbedObjectOriginalScale, 0.25f);
        LeanTween.move(this.grabbedObject, transform.position + Vector3.up * 2f, 0.5f).setOnComplete(() => {
            this.possessable.SetCanMove(true);

            this.grabbedObjRB.isKinematic = false;
            this.grabbedObjRB.angularVelocity = Vector3.zero;
            this.grabbedObjRB.linearVelocity = Vector3.zero;
            this.grabbedObjRB = null;

            this.grabbedObject = null;
        });

        // Trigger the "Grab" animation
        animator.SetTrigger("Release");

        this.throwTimer = this.throwCooldown;
    }

    private void GrabPlayer()
    { //Only happens for the AI
      
        Player.Instance.gameObject.transform.position = transform.position + Vector3.up;
        Player.Instance.SetGrabbed(true);

        // Trigger the "Grab" animation
        animator.SetTrigger("Grab");
    }

    private void ReleasePlayer()
    { //Only happens for the AI
      
        Player.Instance.SetGrabbed(false);
        this.currentGrabCooldown = this.grabCooldown;

        // Trigger the "Grab" animation
        animator.SetTrigger("Release");
    }
}
