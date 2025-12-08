using UnityEngine;
using static MultiTag;

public class Puzzle_ToggleAnimated : SpellActivatable
{
    [Header("Optional lock player, since they cannot interact with ANIMATED alterables.")]
    [SerializeField] bool lockPlayerDuringAnimation = false;
    [SerializeField] float lockTime = 1.5f;

    [Header("Other stuff")]
    public bool isActivated = false;
    [SerializeField] private AK.Wwise.Event initialMoveSound, revertMoveSound;

    [Header("Optional Animator not on this object, otherwise it defaults to self")]
    [SerializeField] Animator animator;


    private void Start()
    {
        if(animator == null)
            animator = GetComponent<Animator>();
    }

    public override void ActivateBySpell()
    {
        Debug.Log("Spell Activated");
        isActivated = !isActivated;
        animator.SetBool("isActivated", isActivated);

        if (lockPlayerDuringAnimation)
        {
            Player.Instance.DisableControls(null);
            Invoke("UnlockPlayer", lockTime);
        }

        if (isActivated) SfxManager.Instance.PostEvent(initialMoveSound, gameObject);
        else SfxManager.Instance.PostEvent(revertMoveSound, gameObject);
    }

    private void UnlockPlayer()
    {
        Player.Instance.EnableControls();
    }
}
