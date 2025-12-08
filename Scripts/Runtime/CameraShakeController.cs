using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using static CompactMath;

public class CameraShakeController : MonoBehaviour {
    
    public static CameraShakeController Instance;
    
    private CinemachineImpulseSource _impulseSource;
    private Coroutine _activeRoutine;


    private void Awake() {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

    }

    private void Start() {
        _impulseSource = Player.Instance.GetComponentInChildren<CinemachineImpulseSource>();
    }


    public void StartCameraShake(float time) {
        
        if (_activeRoutine != null)
            StopCoroutine(_activeRoutine);
        
        _activeRoutine = StartCoroutine(CameraShakeRoutine(time));
    }

    public void StopCameraShake() {
        if (_activeRoutine != null)
            StopCoroutine(_activeRoutine);
        
        _impulseSource.CancelInvoke();
    }

    private IEnumerator CameraShakeRoutine(float time) {
        float times = time / 0.2f;
        times = (int)times;

        for (int i = 0; i < times; i++) {
            _impulseSource.GenerateImpulse(AbsVector3(0.1f, 0.05f));
            yield return new WaitForSeconds(0.2f);
        }
    }
}
