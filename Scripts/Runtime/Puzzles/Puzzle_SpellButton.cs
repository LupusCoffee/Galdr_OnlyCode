using UnityEngine;

public class Puzzle_SpellButton : SpellActivatable
{
    public override void ActivateBySpell()
    {
        base.ActivateBySpell();

        Debug.Log("Spell button activated");
        TriggerOnButtonPressed();
    }
}
