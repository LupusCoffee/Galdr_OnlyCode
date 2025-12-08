//Created by Mohammed

using System;
using System.Collections.Generic;
using UnityEngine;
using static CompactMath;
using static MultiTag;

public class AbilityBehaviour
{
    bool spellEffective;

    LayerMask spellLayerMask = 1 << 0;
    public enum SpellEffect
    {
        NONE,
        AWAKEN, // awaken something - machina engine for a period of time, my dick
        POSSESS, // take control of an enemy and move them
        ALTER // raise objects, move them, open door, special buttons
    }

    public enum SpellTarget
    {
        NONE,
        CORRUPTION, // enemies, corruption, mushrooms
        NATURE, // moving things like elevators, doors, etc
        MACHINA // manmade objects, buttons, etc
    }

    public bool Activate(SpellEffect ability, SpellTarget target, GameObject gameObjectToActivate)
    {
        spellEffective = false; //bruh

        Debug.Log("Activating ability: " + ability + " on target: " + target);
        //ability behaviours are implemented here
        switch (ability)
        {
            case SpellEffect.AWAKEN:

                //awaken spell
                Spell_Awaken(target, gameObjectToActivate);

                break;

            case SpellEffect.POSSESS:

                Spell_Possess(target, gameObjectToActivate);

                break;

            case SpellEffect.ALTER:

                Spell_Alter(target, gameObjectToActivate);

                break;

            default:
                break;
        }

        return spellEffective;
    }

    #region Ability Behaviours
    private void Spell_Awaken(SpellTarget target, GameObject hitObject)
    {
        /*GameObject hitObject = null;
        MultiTags tag = MultiTags.Awakenable;

        if (!SpellTargetFinder.GetTarget(tag, out hitObject))
            return;*/

        if (hitObject.TryGetComponent(out Awakenable awakenable))
        {
            awakenable.SpellHit();
            spellEffective = true; //bruh
        }
    }

    private void Spell_Possess(SpellTarget target, GameObject hitObject)
    {
        /*GameObject hitObject = null;
        MultiTags tag = MultiTags.Possessable;

        if (!SpellTargetFinder.GetTarget(tag, out hitObject))
            return;*/

        if (hitObject.TryGetComponent(out Possessable possess))
        {
            spellEffective = true; //bruh
            Player.Instance.DisableControls(hitObject);
            possess.ActivateMindControl();
            Debug.Log("Mind controlled object");
        }
    }

    private void Spell_Alter(SpellTarget target, GameObject hitObject)
    {
        /*GameObject hitObject = null;
        MultiTags tag = MultiTags.Alterable;
        /*if (!SpellTargetFinder.GetTarget(tag, out hitObject))
            return;*/

        if (hitObject.TryGetComponent(out Alterable alterable))
        {
            alterable.SpellHit();
            spellEffective = true; //bruh
        }

    }
    #endregion

    #region Ability Implementations
    private MultiTags[] ConvertTypeToMultiTag(SpellTarget target)
    {
        Debug.Log("Converting target to tag: " + target);
        switch (target)
        {
            case SpellTarget.CORRUPTION:
                return new MultiTags[] { MultiTags.Corruption };
            case SpellTarget.NATURE:
                return new MultiTags[] { MultiTags.Nature, MultiTags.Alterable };
            case SpellTarget.MACHINA:
                return new MultiTags[] { MultiTags.Machina };
            default:
                return new MultiTags[] { MultiTags.None };
        }
    }

    public GameObject GetSingleTarget(SpellTarget type, float overrideRadius = 1, float overrideRange = 10)
    {
        //first check below player if they are standing on something
        Collider[] hitColliders = Physics.OverlapSphere(Player.Instance.transform.position, 1.25f, spellLayerMask);

        foreach (Collider hitCollider in hitColliders)
        {
            if (HasTag(hitCollider.gameObject, ConvertTypeToMultiTag(type)))
            {
                return hitCollider.gameObject;
            }
        }

        //spherecast forward in player direction
        RaycastHit hitForward;
        Debug.DrawLine(Player.Instance.transform.position, Player.Instance.transform.position + Player.Instance.transform.forward * overrideRange, Color.red, 5);

        Vector3 heightModifier = new Vector3(0, -0.15f, 0);
        if (Physics.SphereCast(Player.Instance.transform.position - heightModifier, overrideRadius, Player.Instance.transform.forward, out hitForward, 10, spellLayerMask))
        {
            if (HasTag(hitForward.collider.gameObject, ConvertTypeToMultiTag(type)))
            {
                return hitForward.collider.gameObject;
            }
            else
            {
                Debug.Log("No matching tag found");
            }
        }

        return null;
    }

    //public GameObject GetSingleTarget(MultiTags tag)
    //{
    //    //first check below player if they are standing on something
    //    Collider[] hitColliders = Physics.OverlapSphere(Player.Instance.transform.position, 1.25f, spellLayerMask);

    //    foreach (Collider hitCollider in hitColliders)
    //    {
    //        if (HasTag(hitCollider.gameObject, tag))
    //        {
    //            return hitCollider.gameObject;
    //        }
    //    }

    //    //spherecast forward in player direction
    //    RaycastHit hitForward;
    //    if (Physics.SphereCast(Player.Instance.transform.position, 1, Player.Instance.transform.forward, out hitForward, 10, spellLayerMask))
    //    {
    //        if (HasTag(hitForward.collider.gameObject, tag))
    //        {
    //            return hitForward.collider.gameObject;
    //        }
    //        else
    //        {
    //            Debug.Log("No matching tag found");
    //        }
    //    }

    //    return null;
    //}

    public List<GameObject> GetAreaTargets(SpellTarget type, float range)
    {
        // get everything in an area
        Collider[] hitColliders = Physics.OverlapSphere(Player.Instance.transform.position, range);

        List<GameObject> targetObjects = new List<GameObject>();

        foreach (Collider hitCollider in hitColliders)
        {
            if (HasTag(hitCollider.gameObject, ConvertTypeToMultiTag(type)))
            {
                targetObjects.Add(hitCollider.gameObject);
            }
        }

        return targetObjects;
    }
    #endregion
}
