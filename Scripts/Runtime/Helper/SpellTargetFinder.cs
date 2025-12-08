using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CompactMath;
using static MultiTag;

public static class SpellTargetFinder
{
    private static readonly LayerMask spellLayerMask = 1 << 0;
    private static Transform origin = Player.Instance.transform;

    public static void Init()
    {
        origin = Player.Instance.transform;
    }

    #region Spell Target

    public static bool GetAllForwardMultiTags(out List<MultiTag> multiTags)
    {
        multiTags = null;
        
        
        
        return false;
    }
    
    public static bool GetTarget(MultiTags tag, out GameObject hitObject)
    {
        hitObject = null;

        if (TryFindProximityTarget(tag, out hitObject) || TryFindForwardTarget(tag, out hitObject))
        {
            return hitObject;
        }

        return hitObject;
    }

    private static bool TryFindProximityTarget(MultiTags tag, out GameObject hitObject)
    {
        hitObject = null;

        // Raycast straight down to detect targets under the player
        if (Physics.Raycast(origin.position, Vector3.down, out RaycastHit hit, 1.5f, spellLayerMask))
        {
            if (HasTag(hit.collider.gameObject, tag))
            {
                hitObject = hit.collider.gameObject;
                return true;
            }
        }

        return false;
    }

    private static bool TryFindForwardTarget(MultiTags tag, out GameObject closestObject)
    {
        closestObject = null;
        Vector3 originPosition = origin.position + new Vector3(0, -0.15f, 0); // Adjusted height
        RaycastHit[] raycastHits = Physics.SphereCastAll(originPosition, 2f, origin.forward, 10, spellLayerMask);
        List<(GameObject hitObject, float distance)> distanceList = new List<(GameObject, float)>();

        foreach (RaycastHit hit in raycastHits)
        {
            if (HasTag(hit.collider.gameObject, tag))
            {
                float distance = Vector3.SqrMagnitude(origin.position - hit.collider.transform.position);
                distanceList.Add((hit.collider.gameObject, distance));
            }
        }

        // Sorted items by distance
        foreach (var item in distanceList.OrderBy(x => x.distance))
        {
            GameObject potentialTarget = item.hitObject;
            RaycastHit raycastHit;

            Collider targetCollider = potentialTarget.GetComponent<Collider>();

            if (targetCollider != null)
            {
                // Get the bounds of the target's collider
                Bounds bounds = targetCollider.bounds;


                // this checks if any part of the target is visible
                Vector3[] pointsToCheck = new Vector3[]
                {
                    bounds.center,
                    bounds.min,
                    bounds.max,
                    new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
                    new Vector3(bounds.max.x, bounds.max.y, bounds.min.z)
                };

                foreach (Vector3 point in pointsToCheck)
                {
                    Vector3 directionToTarget = point - originPosition;

                    if (Physics.Raycast(originPosition, directionToTarget.normalized, out raycastHit, directionToTarget.magnitude, spellLayerMask, QueryTriggerInteraction.Ignore))
                    {
                        if (raycastHit.collider.gameObject == potentialTarget)
                        {
                            closestObject = potentialTarget;
                            return true;
                        }
                    }
                }
            }

        }

        Debug.Log("No target found forward");
        return false;
    }
    #endregion



    #region Tag Conversion
    private static readonly Dictionary<AbilityBehaviour.SpellTarget, MultiTags> TargetTagMappings = new()
    {
        { AbilityBehaviour.SpellTarget.NATURE, MultiTags.Nature },
        { AbilityBehaviour.SpellTarget.CORRUPTION, MultiTags.Corruption },
        { AbilityBehaviour.SpellTarget.MACHINA, MultiTags.Machina },
    };

    private static readonly Dictionary<AbilityBehaviour.SpellEffect, MultiTags> TypeTagMappings = new()
    {
        { AbilityBehaviour.SpellEffect.ALTER, MultiTags.Alterable },
        { AbilityBehaviour.SpellEffect.POSSESS, MultiTags.Possessable },
        { AbilityBehaviour.SpellEffect.AWAKEN, MultiTags.Awakenable }
    };

    public static MultiTags ConvertTypeToMultiTag(AbilityBehaviour.SpellEffect effect)
    {
        if (TypeTagMappings.TryGetValue(effect, out MultiTags tag))
        {
            return tag;
        }
        return MultiTags.None;
    }
    
    public static MultiTags ConvertTypeToMultiTag(AbilityBehaviour.SpellTarget target)
    {
        if (TargetTagMappings.TryGetValue(target, out MultiTags tag))
        {
            return tag;
        }
        return MultiTags.None;
    }
    #endregion
}
