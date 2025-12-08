using UnityEngine;

public class MultiTag : MonoBehaviour
{
    [System.Flags]
    public enum MultiTags
    {
        None = 0,
        Weighted = 1, // affects objects in scene such as pressure plates
        Floating = 2, // does not affect objects in scene such as pressure plates, is floating
        Possessable = 4, // is affected by the POSSESS ability
        Awakenable = 8, // is affected by the AWAKE ability
        Corruption = 16, // belongs to the CORRUPTION category
        Alterable = 32, // is affected by the ALTER ability
        Illuminable = 64, // is affected by the ILLUMINATE ability
        Nature = 128, // belongs to the NATURE category
        Pickups = 256, // is a pickup object
        Player = 512, // is the player
        Possess_Grabbable = 1024, // is grabbable by the player when possessing an enemy
        Machina = 2048, // belongs to the MACHINA category
    }

    public MultiTags tags;

    //check if the object has a specific tag
    public bool HasTag(MultiTags tag)
    {
        return (tags & tag) == tag;
    }

    public static MultiTag TryGetMultitag(GameObject obj)
    {
        return obj.GetComponent<MultiTag>();
    }

    public static bool HasTag(GameObject obj, MultiTags tag)
    {
        MultiTag mt = TryGetMultitag(obj);
        if (mt == null)
        {
            return false;
        }
        return mt.HasTag(tag);
    }

    public static bool HasTag(GameObject obj, MultiTags[] tags)
    {
        MultiTag mt = TryGetMultitag(obj);
        if (mt == null)
        {
            return false;
        }
        foreach (MultiTags tag in tags)
        {
            if (mt.HasTag(tag))
            {
                return true;
            }
        }
        return false;
    }
}
