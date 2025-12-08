using System.Collections.Generic;
using UnityEngine;

public class ProximityHelper : MonoBehaviour
{
    public int GetClosestObjectIndex(List<GameObject> gameObjects, Vector3 targetClosestTo)
    {
        int closestIndex = -1;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < gameObjects.Count; i++)
        {
            float distanceToTarget = Vector3.Distance(targetClosestTo, gameObjects[i].transform.position);
            
            if (closestDistance < distanceToTarget) continue;
            
            closestIndex = i;
            closestDistance = distanceToTarget;
        }
        
        return closestIndex;
    }
    
    public GameObject GetClosestGameObject(List<GameObject> gameObjects, Vector3 targetClosestTo)
    {
        GameObject closestGameObject = null;
        float closestDistance = float.MaxValue;
        
        foreach (var gameObject in gameObjects)
        {
            float distanceToTarget = Vector3.Distance(targetClosestTo, gameObject.transform.position);

            if (closestDistance < distanceToTarget) continue;
            
            closestGameObject = gameObject;
            closestDistance = distanceToTarget;
        }
        
        return closestGameObject;
    }
    
    public SpellCastableObject GetClosestSpellCastableObject(List<SpellCastableObject> spellCastableObjects, Vector3 targetClosestTo)
    {
        SpellCastableObject closestSpellCastableObject = null;
        float closestDistance = float.MaxValue;
        
        foreach (var spellCastableObject in spellCastableObjects)
        {
            float distanceToTarget = Vector3.Distance(targetClosestTo, spellCastableObject.transform.position);

            if (closestDistance < distanceToTarget) continue;
            
            closestSpellCastableObject = spellCastableObject;
            closestDistance = distanceToTarget;
        }
        
        return closestSpellCastableObject;
    }
}
