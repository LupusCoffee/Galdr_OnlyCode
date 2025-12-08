using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static MultiTag;
using static CompactMath;
using System.Collections;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public class Puzzle_Movable : SpellActivatable
{
    public enum ActivationType { SPELL, BUTTON };

    public enum RotationType { TOGGLED, ADDITIVE }
    [SerializeField] private GameObject soundEmitter;
    [SerializeField] private AK.Wwise.Event oneTimeEventOnInteraction;
    [SerializeField] private AK.Wwise.Event initialMoveSound, revertMoveSound;
    [SerializeField] private AK.Wwise.Event constantInitialMoveSoundPlay, constantRevertMoveSoundPlay;
    [SerializeField] private AK.Wwise.Event constantInitialMoveSoundStop, constantRevertMoveSoundStop;
    bool oneTimeEventHappened = false;

    private Vector3 initialPosition;
    private Vector3 initialScale;
    private Vector3 initialRotation;

    private Vector3 alternatePosition;

    [SerializeField] bool alterPosition = false;
    [SerializeField] bool alterRotation = false;
    [SerializeField] bool alterScale = false;

    [SerializeField] private Vector3 alternatePositionOffset;
    [SerializeField] private Vector3 alternateRotation;
    [SerializeField] private float alternateScale = 1f;

    [SerializeField] private RotationType rotationType = RotationType.TOGGLED;

    [SerializeField] float moveSpeed = 1f;
    [SerializeField] bool useCameraShake = false;

    [Header("Activation Settings")]
    [SerializeField] public ActivationType activationType = ActivationType.SPELL;
    [SerializeField] PuzzleEvent activateEvent;
    [SerializeField] bool listenOnlyOnce = false;

    [Header("Collider Settings")]
    [SerializeField] bool enableColliderOnActivation = false;
    [SerializeField] Collider colliderToEnable;

    bool isAltered = false;
    bool isMoved = false;

    bool movementInProgress;

    Task cancelTask = null;

    private void Awake()
    {
        LeanTween.reset();

        if (activationType == ActivationType.BUTTON && activateEvent != null)
        {
            activateEvent.OnButtonPressed += SetUnaltered;
            activateEvent.OnButtonReleased += SetAltered;
        }
    }

    private void Start()
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;
        initialRotation = transform.rotation.eulerAngles;
        alternatePosition = initialPosition + alternatePositionOffset;

        if (enableColliderOnActivation)
        {
            colliderToEnable.enabled = false;
        }
    }

    public override void ActivateBySpell()
    {
        print("object go!!");
        
        if (!enabled) return;

        if (movementInProgress || activationType == ActivationType.BUTTON) return;

        HandleMovement();
    }

    public void SetAltered(GameObject obj)
    {
        isAltered = true;

        HandleMovement(true);
    }

    public void SetUnaltered(GameObject obj)
    {
        isAltered = false;

        HandleMovement(true);
    }

    private void HandleMovement(bool forceCancel = false)
    {
        Debug.Log("HandleMovement");

        if (movementInProgress && !forceCancel) return;
        if (listenOnlyOnce && isMoved) return;

        isMoved = true;

        LeanTween.cancel(gameObject);

        StartCoroutine(MovementTimer());

        if (isAltered)
        {
            if (soundEmitter != null)
            {
                SfxManager.Instance.PostEvent(revertMoveSound, soundEmitter);
                SfxManager.Instance.PostEvent(constantRevertMoveSoundPlay, soundEmitter);
                SfxManager.Instance.PostEvent(constantInitialMoveSoundStop, soundEmitter);
            }
            else
            {
                SfxManager.Instance.PostEvent(revertMoveSound, gameObject);
                SfxManager.Instance.PostEvent(constantRevertMoveSoundPlay, gameObject);
                SfxManager.Instance.PostEvent(constantInitialMoveSoundStop, gameObject);
            }


            if (alterPosition) LeanTween.move(gameObject, initialPosition, moveSpeed).setOnComplete(() =>
            {
                if (soundEmitter != null)
                    SfxManager.Instance.PostEvent(constantRevertMoveSoundStop, soundEmitter);
                else
                    SfxManager.Instance.PostEvent(constantRevertMoveSoundStop, gameObject);
            });
            if (alterScale) LeanTween.scale(gameObject, initialScale, moveSpeed);

            if (enableColliderOnActivation)
            {
                colliderToEnable.enabled = false;
            }
        }
        else
        {
            PlayOneTimeEvent(oneTimeEventOnInteraction, gameObject);
            if (soundEmitter != null)
            {
                SfxManager.Instance.PostEvent(initialMoveSound, soundEmitter);
                SfxManager.Instance.PostEvent(constantInitialMoveSoundPlay, soundEmitter);
                SfxManager.Instance.PostEvent(constantRevertMoveSoundStop, soundEmitter);
            }
            else
            {
                SfxManager.Instance.PostEvent(initialMoveSound, gameObject);
                SfxManager.Instance.PostEvent(constantInitialMoveSoundPlay, gameObject);
                SfxManager.Instance.PostEvent(constantRevertMoveSoundStop, gameObject);
            }

            if (alterPosition) LeanTween.move(gameObject, alternatePosition, moveSpeed).setOnComplete(() =>
            {
                if (soundEmitter != null)
                    SfxManager.Instance.PostEvent(constantInitialMoveSoundStop, soundEmitter);
                else
                    SfxManager.Instance.PostEvent(constantInitialMoveSoundStop, gameObject);
            });
            if (alterScale) LeanTween.scale(gameObject, initialScale * alternateScale, moveSpeed);

            if (enableColliderOnActivation)
            {
                colliderToEnable.enabled = true;
            }
        }
        if (useCameraShake)
            CameraShakeController.Instance.StartCameraShake(moveSpeed);

        if (alterRotation) HandleRotation(isAltered);

        isAltered = !isAltered;
    }


    private void HandleRotation(bool isMoved)
    {
        if (rotationType == RotationType.TOGGLED)
        {
            if (isMoved) {
                LeanTween.rotate(gameObject, initialRotation, moveSpeed);
            }
            else {
                LeanTween.rotate(gameObject, alternateRotation, moveSpeed);
            }
        }
        else if (rotationType == RotationType.ADDITIVE)
        {
            LeanTween.rotate(gameObject, transform.eulerAngles + alternateRotation, moveSpeed);
        }
    }

    IEnumerator MovementTimer()
    {
        movementInProgress = true;
        yield return new WaitForSeconds(moveSpeed);
        movementInProgress = false;
    }

    private void OnDestroy()
    {
        LeanTween.cancel(gameObject);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (HasTag(collision.gameObject, MultiTags.Weighted))
        {
            //collision.gameObject.transform.SetParent(transform, true);
            collision.gameObject.transform.SetParent(transform, true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (HasTag(collision.gameObject, MultiTags.Weighted))
        {
            //collision.gameObject.transform.SetParent(transform, true);
            collision.gameObject.transform.SetParent(null, true);
        }
    }


    private void PlayOneTimeEvent(AK.Wwise.Event oneTimeEvent, GameObject _gameObject)
    {
        if (!oneTimeEventHappened)
        {
            SfxManager.Instance.PostEvent(oneTimeEvent, _gameObject);
            oneTimeEventHappened = true;
        }
    }
}
