using System.Threading.Tasks;
using UnityEngine;
using static CompactMath;
using static MultiTag;

public class Alterable : SpellTarget
{
    protected bool isAltered = false;
    protected bool isMoving = false;
    [SerializeField] protected float moveDuration;
    [SerializeField] protected bool useScreenShake;
    [SerializeField] protected bool onlyMoveOnce;

    [SerializeField] protected AK.Wwise.Event oneTimeEventOnInteraction;
    [SerializeField] protected AK.Wwise.Event initialMoveSound, revertMoveSound;
    [SerializeField] protected AK.Wwise.Event sound_moveLoopStart, sound_revertLoopStart;
    [SerializeField] protected AK.Wwise.Event sound_moveLoopEnd, sound_revertLoopEnd;

    protected virtual void PlayOneTimeEvent()
    {
        if (oneTimeEventHappened) return;

        oneTimeEventHappened = true;
        SfxManager.Instance.TryPostEvent(oneTimeEventOnInteraction, SoundEmitter);
    }

    protected virtual async void PlayMoveSound(float duration)
    {
        AK.Wwise.Event moveSoundEvent = isAltered ? revertMoveSound : initialMoveSound;
        AK.Wwise.Event musicLoopStartEvent = isAltered ? sound_revertLoopStart : sound_moveLoopStart;
        AK.Wwise.Event musicLoopEndEvent = isAltered ? sound_revertLoopEnd : sound_moveLoopEnd;

        SfxManager.Instance.TryPostEvent(moveSoundEvent, SoundEmitter);
        SfxManager.Instance.TryPostEvent(musicLoopStartEvent, SoundEmitter);
        SfxManager.Instance.TryPostEvent(musicLoopEndEvent, SoundEmitter);

        await Task.Delay(SecondsToMilli(duration));
        StopMoveSound();
    }

    protected virtual void StopMoveSound()
    {
        AK.Wwise.Event soundEvent = isAltered ? sound_revertLoopEnd : sound_moveLoopEnd;
        SfxManager.Instance.TryPostEvent(soundEvent, SoundEmitter);
    }

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

    protected async void MoveTimer()
    {
        isMoving = true;
        await Task.Delay(SecondsToMilli(moveDuration));
        isMoving = false;
    }
}
