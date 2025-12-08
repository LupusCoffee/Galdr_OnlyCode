using UnityEngine;
using static MultiTag;

public class Awakenable : SpellTarget
{
    protected bool isAwakened = false;

    [SerializeField] protected float awakeTime = 1;
    protected float awakeTimer = 0;

    protected void OnCollisionStay(Collision collision)
    {
        if (HasTag(collision.gameObject, MultiTags.Weighted))
        {
            collision.gameObject.transform.SetParent(transform, true);
        }
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (HasTag(collision.gameObject, MultiTags.Weighted))
        {
            collision.gameObject.transform.SetParent(null, true);
        }
    }
}
