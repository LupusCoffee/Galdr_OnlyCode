using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using static CompactMath;

public class SpawnWindEffects : MonoBehaviour {
    
    [SerializeField] private GameObject windParticleOne;
    [SerializeField] private GameObject windParticleTwo;
    [SerializeField] private GameObject windParticleThree;
    [SerializeField] private int maxParticles = 15;
    [SerializeField] private float spawnAreaDistance = 40f;
    [SerializeField] private float timeBetweenSpawn = 60f;

    private List<GameObject> _particlesInWorld;

    private void Awake() {
        _particlesInWorld = new List<GameObject>();
    }

    private void Start() {
        //StartCoroutine(SpawnParticles());
        SpawnWindParticles();
    }

    private GameObject RandomParticle() {
        int random = Random.Range(0, 3);
        GameObject randomParticle = windParticleThree;
        
        if (random == 0) {
            randomParticle = windParticleOne;
        }
        
        if (random == 1) {
            randomParticle = windParticleTwo;

        }
        if(random == 2) {
            randomParticle = windParticleThree;
        }

        return randomParticle;
    }

    private IEnumerator DestroyParticles() {
        yield return new WaitForSeconds(timeBetweenSpawn);
        foreach (var particle in _particlesInWorld) {
            Destroy(particle);
        }
        yield return new WaitForSeconds(1f);
        _particlesInWorld.Clear();

        SpawnWindParticles();
    }

    private void SpawnWindParticles() {
        for (int i = 0; i < maxParticles; i++) {
            
            GameObject particle = Instantiate(RandomParticle(), new Vector3(transform.position.x + AbsRange(spawnAreaDistance), Random.Range(5, 15), 
                transform.position.z + AbsRange(spawnAreaDistance)), Quaternion.identity, this.transform);
            _particlesInWorld.Add(particle);
        }

        StartCoroutine(DestroyParticles());
    }

    private void OnDrawGizmos() {
        Vector3 size = new Vector3(transform.position.x + spawnAreaDistance, 5,
            transform.position.z + spawnAreaDistance);
        Gizmos.DrawWireCube(transform.position, size);
    }
}
