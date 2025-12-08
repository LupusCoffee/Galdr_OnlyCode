//Created by Mohammed (the sex god)

using UnityEngine;
using UnityEngine.SceneManagement;

public class Amb2dZoneController : AudioZoneController
{
    [SerializeField] private float radius;
    bool updatingRadiusVolume;


    protected override void EnterZone()
    {
        base.EnterZone();
        updatingRadiusVolume = true;
    }
    protected override void ExitZone()
    {
        base.ExitZone();
        updatingRadiusVolume = false;
    }


    private void Update()
    {
        if(updatingRadiusVolume) //update volume along the radius from edge of collider
        {
            //to-do: update radius from edge of collider
        }
    }
}
