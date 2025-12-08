using UnityEngine;

public class Possess_Thornshell : Possessable
{
    public override void ActivateMindControl()
    {
        base.ActivateMindControl();
        animator.SetBool("HornsActive", true);

        animator.SetBool("IsMoving", true);
    }

    public override void DeactivateMindControl()
    {
        base.DeactivateMindControl();
        animator.SetBool("HornsActive", false);

        animator.SetBool("IsMoving", false);
    }
}
