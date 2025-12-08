using UnityEngine;

public class Alter_EventSender : Alterable
{
    public delegate void OnAltered(bool isAltered);
    public event OnAltered OnAlteredEvent;

    public override void SpellHit()
    {
        if (isAltered && onlyMoveOnce) return;
        isAltered = !isAltered;
        OnAlteredEvent?.Invoke(isAltered);
    }
}
