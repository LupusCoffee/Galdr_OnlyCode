using UnityEngine;

public class Alter_Animated : Alterable
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool onlyMoveOnce = false;

    public override void SpellHit()
    {
        if (isAltered && onlyMoveOnce) return;

        isAltered = !isAltered;

        PlayMoveSound(moveDuration);

        if (useScreenShake) CameraShakeController.Instance.StartCameraShake(moveDuration);

        animator.SetBool("isActivated", isAltered);
    }
}
