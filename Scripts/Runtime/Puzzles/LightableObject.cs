using System.Collections;
using UnityEngine;

public class LightableObject : MonoBehaviour
{
    [SerializeField] Light lightSource;

    Coroutine deactivateCoroutine;

    public void ActivateLight(int length)
    {
        lightSource.enabled = true;
        if (deactivateCoroutine != null) {
            StopCoroutine(deactivateCoroutine);
        }

        deactivateCoroutine = StartCoroutine(DeactivateCoroutine(length));
    }

    IEnumerator DeactivateCoroutine(int length)
    {
        yield return new WaitForSeconds(length);
        lightSource.enabled = false;
    }
}
