using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static MultiTag;

public class Puzzle_PressurePlate : PuzzleEvent
{
    [SerializeField] private AK.Wwise.Event constantInitialMoveSoundPlay, constantRevertMoveSoundPlay;
    [SerializeField] private AK.Wwise.Event constantInitialMoveSoundStop, constantRevertMoveSoundStop;

    [SerializeField] GameObject visuals;
    Vector3 startPos;
    [SerializeField] float sinkDepth = 0.199f;

    [SerializeField] bool isPressed = false;

    [SerializeField] List<Collider> objectsInRange = new List<Collider>();

    [SerializeField] bool hasObjectsInRange;

    float checkCollisionTimer = 1;

    float baseVisualHeight = 0;

    private void Awake()
    {
        startPos = transform.position;
        baseVisualHeight = visuals.transform.localPosition.y;
    }

    private void Update()
    {
        if (objectsInRange.Count > 0)
        {
            checkCollisionTimer -= Time.deltaTime;

            if (checkCollisionTimer <= 0)
            {
                checkCollisionTimer = 1;
                UpdateCollisions();
            }
        }
        else
        {
            if (isPressed)
                CheckRemainingObjects();
        }
    }

    private void UpdateCollisions()
    {
        List<GameObject> tempObjectsToKeep = new List<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.1f);

        // Check if they match the target specifications
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player") || HasTag(collider.gameObject, MultiTags.Weighted))
            {
                tempObjectsToKeep.Add(collider.gameObject);
            }
        }

        //Cleanse objects in range that are not in the temp list
        for (int i = 0; i < objectsInRange.Count; i++)
        {
            if (!tempObjectsToKeep.Contains(objectsInRange[i].gameObject))
            {
                objectsInRange.Remove(objectsInRange[i]);
            }
        }

        CheckRemainingObjects();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || HasTag(other.gameObject, MultiTags.Weighted))
        {
            if (!objectsInRange.Contains(other))
            {
                objectsInRange.Add(other);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || HasTag(other.gameObject, MultiTags.Weighted))
        {
            if (!objectsInRange.Contains(other))
                objectsInRange.Add(other);

            isPressed = true;

            //START move DOWN sfx
            SfxManager.Instance.PostEvent(constantRevertMoveSoundPlay, gameObject);
            //STOP move UP sfx
            SfxManager.Instance.PostEvent(constantInitialMoveSoundStop, gameObject);

            LeanTween.moveLocalY(visuals, baseVisualHeight - sinkDepth, 0.25f).setOnComplete(() =>
            {
                if (objectsInRange.Count >= 1)
                {
                    TriggerOnButtonPressed();
                    //STOP move DOWN sfx
                    SfxManager.Instance.PostEvent(constantRevertMoveSoundStop, gameObject);
                }
            });
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player") || HasTag(other.gameObject, MultiTags.Weighted))
        {
            //START move UP sfx
            //STOP move DOWN sfx
            SfxManager.Instance.PostEvent(constantInitialMoveSoundPlay, gameObject);
            SfxManager.Instance.PostEvent(constantRevertMoveSoundStop, gameObject);

            if (objectsInRange.Contains(other))
                objectsInRange.Remove(other);

            CheckRemainingObjects();
        }
    }

    public void ForceRemoveObjectFromRange(GameObject gameObject)
    {
        if (objectsInRange.Contains(gameObject.GetComponent<Collider>()))
        {
            objectsInRange.Remove(gameObject.GetComponent<Collider>());
        }

        CheckRemainingObjects();
    }

    private void CheckRemainingObjects()
    {
        if (objectsInRange.Count == 0)
        {
            isPressed = false;
            LeanTween.moveLocalY(visuals, baseVisualHeight, 0.25f).setOnComplete(() =>
            {
                TriggerOnButtonReleased();
                //STOP move UP sfx
                SfxManager.Instance.PostEvent(constantInitialMoveSoundStop, gameObject);
            });
        }
    }
}
