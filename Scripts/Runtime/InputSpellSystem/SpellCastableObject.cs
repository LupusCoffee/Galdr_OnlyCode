using System;
using UnityEngine;

[Serializable]
public class SpellCastableObject : MonoBehaviour
{
    private MultiTag multiTag = null;
    private OutlineObject outlineObject = null;

    private void Start()
    {
        multiTag = gameObject.GetComponent<MultiTag>();
        outlineObject = gameObject.GetComponent<OutlineObject>();
    }
    
    public bool TryGetMultiTag(out MultiTag _multiTag)
    {
        _multiTag = multiTag;
        
        if(!_multiTag) return false;
        return true;
    }
    public bool TryGetOutlineObject(out OutlineObject _outlineObject)
    {
        _outlineObject = outlineObject;
        
        if(!_outlineObject) return false;
        return true;
    }
}
