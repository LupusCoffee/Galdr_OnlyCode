using UnityEngine;

public class SpellActivatable : PuzzleEvent
{
    protected bool _isEnabled = false;
    protected bool _canBeDisabled = true;

    public virtual void ActivateBySpell()
    {
        Debug.Log("Spell Activated");
    }
}
