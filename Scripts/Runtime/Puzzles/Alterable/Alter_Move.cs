using System;
using UnityEngine;

public class Alter_Move : Alterable
{
    // Options
    [SerializeField] private bool alterPosition;
    [SerializeField] private bool alterRotation;
    [SerializeField] private bool alterScale;

    // Values
    [SerializeField] private Vector3 alternatePositionOffset;
    [SerializeField] private Vector3 alternateRotationOffset;
    [SerializeField] private float alternateScaleOffset;

    // Extras
    [SerializeField] private bool isAdditive;

    // Collider
    [SerializeField] private bool enableColliderOnActivation;
    [SerializeField] private Collider colliderToEnable;

    private Vector3 initialPosition;
    private Vector3 initialRotation;
    private float initialScale;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.eulerAngles;
        initialScale = transform.localScale.x;
    }

    public override void SpellHit()
    {
        if (isMoving) return;

        if (isAltered && onlyMoveOnce) return;

        PlayOneTimeEvent();
        PlayMoveSound(moveDuration);

        if (useScreenShake) CameraShakeController.Instance.StartCameraShake(moveDuration);

        HandleMove(isAltered);

        MoveTimer();

        isAltered = !isAltered;
    }

    private void HandleMove(bool isAltered)
    {
        if (alterPosition)
            LeanTween.move(gameObject, isAltered ? initialPosition : initialPosition + alternatePositionOffset, moveDuration);

        if (alterScale)
            LeanTween.scale(gameObject, Vector3.one * (isAltered ? initialScale : alternateScaleOffset), moveDuration);

        if (alterRotation)
            HandleRotation(isAltered);

        if (enableColliderOnActivation)
            colliderToEnable.enabled = !isAltered;
    }

    private void HandleRotation(bool isMoved)
    {
        if (!isAdditive)
        {
            Vector3 targetRotation = isMoved ? initialRotation : initialRotation + alternateRotationOffset;
            LeanTween.rotate(gameObject, targetRotation, moveDuration);

            return;
        }

        LeanTween.rotate(gameObject, transform.eulerAngles + alternateRotationOffset, moveDuration);
    }
}
