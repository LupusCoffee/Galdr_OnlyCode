using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

public class Audio_SplineSoundEmitter : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event startSoundToEmit, stopSoundToEmit;

    private SplineContainer splineContainer;

    private float closestPlayerFloat;
    private GameObject emitter;


    private void Start()
    {
        splineContainer = GetComponent<SplineContainer>();
        emitter = new GameObject();

        SfxManager.AkSceneUnloadingEvent soundForUnloading = new SfxManager.AkSceneUnloadingEvent(startSoundToEmit, stopSoundToEmit, emitter);
        SfxManager.Instance.PostStartEventForSceneUnloading(soundForUnloading);
    }

    private void Update()
    {
        if (Player.Instance == null) return;

        float3 playerPos = splineContainer.transform.InverseTransformPoint(Player.Instance.gameObject.transform.position);

        SplineUtility.GetNearestPoint(splineContainer.Spline, playerPos, out float3 localClosestPlayerPoint, out float closestPlayerFloat);

        Vector3 globalClosestPlayerPoint = splineContainer.transform.TransformPoint(localClosestPlayerPoint);

        emitter.transform.position = globalClosestPlayerPoint;
    }

    //TO-DO: if the player is in radius using a trigger, turn on the soundToEmit. If it's out of range, turn it off.
}
