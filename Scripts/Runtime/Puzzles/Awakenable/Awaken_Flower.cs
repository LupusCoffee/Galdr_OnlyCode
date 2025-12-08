using UnityEngine;
using static MultiTag;

public class Awaken_Flower : Awakenable
{
    [SerializeField] ParticleSystem particles;

    private void Start()
    {
        particles.Stop();
    }

    private void Update()
    {
        if (isAwakened)
        {
            awakeTimer += Time.deltaTime;
            if (awakeTimer >= awakeTime)
            {
                particles.Stop();
                isAwakened = false;
                awakeTimer = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (HasTag(other.gameObject, MultiTags.Player) && isAwakened)
        {
            //add force to negate gravity, plus some extra
            other.attachedRigidbody.linearVelocity = Vector3.zero;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (HasTag(other.gameObject, MultiTags.Player) && isAwakened)
        {
            //add force to negate gravity, plus some extra
            other.attachedRigidbody.AddForce(Vector3.up * 10, ForceMode.Force);
        }
    }

    public override void SpellHit()
    {
        if (!isAwakened)
        {
            particles.Play();
            isAwakened = true;
        }
    }
}
