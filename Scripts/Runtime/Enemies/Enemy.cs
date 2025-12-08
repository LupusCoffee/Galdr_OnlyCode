using System.Collections;
using UnityEngine;

public abstract class Enemy : PausableMonoBehaviour {
    protected bool dazed;
    protected float dazeDuration = 1;
    protected Coroutine dazeCoroutine;
    protected Rigidbody rb;

    protected virtual void Start() {
        rb = GetComponentInChildren<Rigidbody>();
    }

    public virtual void PlayerControl_Activate()
    {

    }

    public virtual void PlayerControl_Deactivate()
    {

    }

    public virtual void Daze()
    {
        dazed = true;

        if (dazeCoroutine != null)
            StopCoroutine(dazeCoroutine);

        dazeCoroutine = StartCoroutine(EndDaze(dazeDuration));
    }

    IEnumerator EndDaze(float timer)
    {
        yield return new WaitForSeconds(timer);
        dazed = false;
        rb.linearVelocity = Vector3.zero;
    }

}
