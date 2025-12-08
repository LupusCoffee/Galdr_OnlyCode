using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [Header("Spell VFX")]
    [SerializeField] private GameObject spellCompletedVFX;

    List<GameObject> activeSpellVfx = new List<GameObject>();


    private void Start()
    {
        if(Instance == null) Instance = this;
    }


    public void PlaySpellParticles(GameObject spellParticles, Transform _transform)
    {
        if (spellParticles == null || _transform == null) return;

        GameObject temp = Instantiate(spellParticles, new Vector3(_transform.position.x, _transform.position.y - 1f, _transform.position.z), Quaternion.Euler(spellParticles.transform.eulerAngles));
        temp.transform.SetParent(_transform, true);

        activeSpellVfx.Add(temp);
    }

    public void StopSpellParticles()
    {
        foreach (var item in activeSpellVfx)
            item.GetComponent<ParticleSystem>().Stop();

        activeSpellVfx.Clear();
    }

    public GameObject GetSpellCompletedVFX() => spellCompletedVFX;
}
