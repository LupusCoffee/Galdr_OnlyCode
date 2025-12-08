using UnityEngine;

public class Possess_Devourer : Possessable
{
    protected override void Awake()
    {
        base.Awake();
        animator = GetComponentInChildren<Animator>();
    }

    public override void ActivateMindControl()
    {
        base.ActivateMindControl();

        SfxManager.Instance.PostEvent("TentacleMove");
    }

    public override void DeactivateMindControl()
    {
        base.DeactivateMindControl();

        SfxManager.Instance.PostEvent("TentacleStop");
    }
}
