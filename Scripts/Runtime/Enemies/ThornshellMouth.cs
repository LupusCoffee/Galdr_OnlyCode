using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using static CompactMath;
using static MultiTag;

public class ThornshellMouth : Enemy {
    [SerializeField] float throwStrength = 5;
    [SerializeField] public bool active = false;

    Possessable possessable;

    private List<GameObject> grabbableObjectsInRange = new List<GameObject>();
    GameObject grabbedObject;

    float throwCooldown = 0.5f;
    float throwTimer = 0;

    Transform objToMove;

    SphereCollider grabbedCollider;

    Rigidbody myRigidBody;

    List<Coroutine> throwCoroutines = new List<Coroutine>();

    [SerializeField] Animator animator;

    public override void OnEnable() {
        base.OnEnable();
        UserInputs.Instance._playerInteract.performed += OnInteract;
    }

    public override void OnDisable() {
        base.OnDisable();
        UserInputs.Instance._playerInteract.performed -= OnInteract;
    }

    private void Awake() {
        this.possessable = GetComponentInParent<Possessable>();
        this.objToMove = transform.parent;

        this.grabbedCollider = GetComponentInParent<SphereCollider>();
    }

    private void Update() {
        if (this.throwTimer > 0) {
            this.throwTimer -= Time.deltaTime;
        }

        if (this.possessable.hasBeenPossessed && this.grabbableObjectsInRange.Count > 0 && 
                this.possessable.isMindControlled) {
            InteractPrompt.Instance.ShowInteractPrompt(InteractPrompt.Instance.grabSprite);

            if(GetLastGrabbableObject() != null)
                GetLastGrabbableObject().GetComponent<OutlineObject>().SetOutlinePink(true);

            this.possessable.hasBeenPossessed = false;
        } else if (this.possessable.hasBeenPossessed && this.grabbableObjectsInRange.Count == 0) {
            this.possessable.hasBeenPossessed = false;
        }

        if (this.possessable.hasBeenUnpossessed && this.grabbableObjectsInRange.Count > 0 && 
                !this.possessable.isMindControlled) {
            //check if the player has anything in InteractZone?
            InteractPrompt.Instance.HideInteractPrompt();

            /*foreach (GameObject currentObject in grabbableObjectsInRange) {
                currentObject.GetComponent<OutlineObject>().SetOutlinePink(false);
            }*/

            if (GetLastGrabbableObject() != null)
                GetLastGrabbableObject().GetComponent<OutlineObject>().SetOutlinePink(false);

            this.possessable.hasBeenUnpossessed = false;
        } else if (this.possessable.hasBeenUnpossessed && this.grabbableObjectsInRange.Count == 0) {
            this.possessable.hasBeenUnpossessed = false;
        }

        //move player along with self if attached
        if (this.active) {
            Vector3 grabbedPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            this.grabbedObject.transform.position = grabbedPos - transform.forward * 0.5f;
            this.grabbedObject.transform.rotation = transform.rotation;
        }
    }


    private void OnTriggerEnter(Collider other) {
        if (this.grabbedObject == null && (HasTag(other.gameObject, MultiTags.Player) ||
                HasTag(other.gameObject, MultiTags.Possess_Grabbable))) {
            this.grabbableObjectsInRange.Add(other.gameObject);

            if (this.possessable.isMindControlled) {
                if (GetLastGrabbableObject().TryGetComponent(out OutlineObject newOutlineObject)) {
                    newOutlineObject.SetOutlinePink(true);
                    InteractPrompt.Instance.ShowInteractPrompt(InteractPrompt.Instance.grabSprite);
                }

                if (this.grabbableObjectsInRange.Count > 1 && 
                        this.grabbableObjectsInRange[this.grabbableObjectsInRange.Count-2].
                        TryGetComponent(out OutlineObject oldOutlineObject)) {
                    oldOutlineObject.SetOutlinePink(false);

                } /*else if (this.grabbableObjectsInRange.Count == 0) {
                    //Probably not needed...
                    InteractPrompt.Instance.HideInteractPrompt();
                }*/
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if(this.grabbedObject == null && (HasTag(other.gameObject, MultiTags.Player) ||
                HasTag(other.gameObject, MultiTags.Possess_Grabbable))) {
            this.grabbableObjectsInRange.Remove(other.gameObject);

            if (this.possessable.isMindControlled) {
                if (other.gameObject.TryGetComponent(out OutlineObject oldOutlineObject)) {
                    
                    oldOutlineObject.SetOutlinePink(false);
                }

                if (this.grabbableObjectsInRange.Count > 0 &&
                        GetLastGrabbableObject().TryGetComponent(out OutlineObject nextOutlineObject)) {
                    nextOutlineObject.SetOutlinePink(true);
                } else if (this.grabbableObjectsInRange.Count == 0) {
                    InteractPrompt.Instance.HideInteractPrompt();
                }
            }
        }
    }

    public GameObject GetFirstGrabbableObject() {
        if (this.grabbableObjectsInRange.Count > 0)
            return this.grabbableObjectsInRange[0];
        return null;
    }

    public GameObject GetLastGrabbableObject() {
        if (this.grabbableObjectsInRange.Count > 0)
            return this.grabbableObjectsInRange[this.grabbableObjectsInRange.Count-1];
        return null;
    }

    public void OnInteract(InputAction.CallbackContext ctx) {
        if (!this.possessable.isMindControlled) return;
        if (this.grabbedObject != null || this.grabbableObjectsInRange.Count > 0) {
            if (this.active) {
                PlayerControl_Deactivate();
            } else {
                PlayerControl_Activate();
            }
        }
    }

    public override void PlayerControl_Activate() {
        if(this.grabbableObjectsInRange.Count > 0) {
            this.grabbedObject = GetLastGrabbableObject();
            this.grabbableObjectsInRange.Remove(this.grabbedObject);

            InteractPrompt.Instance.ShowInteractPrompt(InteractPrompt.Instance.throwSprite);

            if (this.grabbedObject.TryGetComponent(out OutlineObject grabbedObject)) {
                grabbedObject.SetOutlinePink(false);
            }

            this.active = true;
            this.grabbedObject.GetComponent<Collider>().enabled = false;
            this.grabbedObject.GetComponent<Rigidbody>().isKinematic = true;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2);

            foreach (Collider hitCollider in hitColliders) {
                if (hitCollider.TryGetComponent(out Puzzle_PressurePlate pressurePlate)) {
                    pressurePlate.ForceRemoveObjectFromRange(this.grabbedObject);
                }
            }
        }

        this.throwTimer = this.throwCooldown;
        this.grabbedCollider.enabled = true;
    }

    public override void PlayerControl_Deactivate() {
        if (this.throwTimer > 0) return;

        this.active = false;

        if (this.grabbableObjectsInRange.Count == 0) {
            InteractPrompt.Instance.HideInteractPrompt();
        } else {
            InteractPrompt.Instance.ShowInteractPrompt(InteractPrompt.Instance.grabSprite);
        }

        if (this.grabbedObject != null) {

            SfxManager.Instance.PostEvent("Play_ThornshellThrow", gameObject);
            this.grabbedObject.GetComponent<Collider>().enabled = true;

            Rigidbody otherRb = grabbedObject.GetComponent<Rigidbody>();
            otherRb.isKinematic = false;
            otherRb.linearVelocity = Vector3.zero;

            Vector3 direction = transform.forward + Vector3.up;

            if (this.grabbedObject.TryGetComponent(out Rigidbody rb)) {
                rb.angularVelocity = Vector3.zero;
                rb.linearVelocity = Vector3.zero;
                rb.AddForce(direction * throwStrength, ForceMode.Impulse);
                animator.SetTrigger("IsThrowing");
                
            }
        }

        this.throwTimer = this.throwCooldown;

        this.grabbedObject = null;
        //grabbableObjectInRange = false;

        this.grabbedCollider.enabled = false;
    }
}
