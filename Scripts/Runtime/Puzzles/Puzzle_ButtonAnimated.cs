using System.Collections;
using UnityEngine;

public class Puzzle_ButtonAnimated : MonoBehaviour
{
    Animator animator;
    public bool isActivated = false;

    bool playOpenDoor = false;
    bool playCloseDoor = false;

    [SerializeField] PuzzleEvent activateButton;
    [SerializeField] private AK.Wwise.Event initialMoveSound, revertMoveSound;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (activateButton != null)
        {
            activateButton.OnButtonPressed += ActivateSelf;
            activateButton.OnButtonReleased += DeactivateSelf;
        }
    }

    public void ActivateSelf(GameObject obj)
    {
        isActivated = true;
        animator.SetBool("isActivated", isActivated);
        //playOpenDoor = true;
        //playCloseDoor = false;
        SfxManager.Instance.PostEvent(initialMoveSound, gameObject);
    }

    public void DeactivateSelf(GameObject obj)
    {
        isActivated = false;
        animator.SetBool("isActivated", isActivated);
        //playCloseDoor = true;
       // playOpenDoor = false;
        SfxManager.Instance.PostEvent(revertMoveSound, gameObject);
    }

    private void OnDestroy()
    {
        if (activateButton != null)
        {
            activateButton.OnButtonPressed -= ActivateSelf;
            activateButton.OnButtonReleased -= DeactivateSelf;
        }
    }


    /* TO-DO: FIIIIIIIIIIIIIIIX THIIIIIIIIIIIIIIIIIIIS AAAAAAAAAAAAAAAAHHHHHHHHHHHHHHHHHHH
    private void Update()
    {
        //BRUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUUHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH
        if(playOpenDoor && animator.GetCurrentAnimatorStateInfo(0).play)
        {
            SfxManager.Instance.PostEvent(initialMoveSound, gameObject);
            playOpenDoor = false;
        }

        if (playCloseDoor && animator.GetCurrentAnimatorStateInfo(0).IsName("Door_Close"))
        {
            SfxManager.Instance.PostEvent(initialMoveSound, gameObject);
            playCloseDoor = false;
        }
    }
    */
}
