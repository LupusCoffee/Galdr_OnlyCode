using UnityEngine;

public class Alter_Enable : Alterable
{
    [SerializeField] GameObject[] objectsToEnable;
    [SerializeField] bool toggleable;

    private bool isActivated;

    public override void SpellHit()
    { 
        if(!toggleable && isActivated) {
            return;
        }

        isActivated = !isActivated;

        foreach (GameObject obj in objectsToEnable)
        {
            obj.SetActive(isActivated);
        }
    }
}
