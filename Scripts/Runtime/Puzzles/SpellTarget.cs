using UnityEngine;

public class SpellTarget : PausableMonoBehaviour
{
    public virtual void SpellHit() { }

    protected bool oneTimeEventHappened = false;

    [SerializeField] protected GameObject soundEmitter;
    protected GameObject SoundEmitter { get => soundEmitter == null ? gameObject : soundEmitter; }
}
