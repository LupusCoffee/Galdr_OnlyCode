using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static MultiTag;

public class Audio_PlayerFootstepsController : MonoBehaviour
{
    enum Ground { GRASS, GRAVEL, STONE }
    Ground currentGround;
    string currentTerrainLayer;

    [SerializeField] private float groundedLength = 0.5f;
    [SerializeField] private float initialDelay;
    [SerializeField] private float currentFootstepSfxFrequency;
    [SerializeField] private float playerFootstepsSfxFrequency;
    [SerializeField] private float thornshellFootstepSfxFrequency;
    [SerializeField] private float devourerFootstepSfxFrequency;
    IEnumerator currentContinousFootsteps;
    bool startIsPlaying = false;

    bool onTerrain = true;
    bool isGrounded = true; //probably needed for falling

    string startStep;
    string continousStep;
    string endStep;


    private void OnEnable()
    {
        UserInputs.Instance._playerMove.performed += StartFootsteps;
        UserInputs.Instance._playerMove.canceled += EndFootsteps;
    }
    private void OnDisable()
    {
        UserInputs.Instance._playerMove.performed -= StartFootsteps;
        UserInputs.Instance._playerMove.canceled -= EndFootsteps;
    }

    private void Awake()
    {
        if (!this.gameObject.CompareTag("Feet")) this.enabled = false;
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundedLength))
        {
            isGrounded = true;

            if (hit.collider.gameObject.CompareTag("Terrain"))
            {
                onTerrain = true;
                //TerrainManager.Instance.SetCurrentTerrain(hit.collider.GetComponent<Terrain>()); //i know this is bad, very sorry
            }
            else onTerrain = false;

            if (hit.collider.gameObject.CompareTag("Grass"))
                currentGround = Ground.GRASS;
            if (hit.collider.gameObject.CompareTag("Gravel"))
                currentGround = Ground.GRAVEL;
            if (hit.collider.gameObject.CompareTag("Stone"))
                currentGround = Ground.STONE;
        }
        else isGrounded = false;

        SetSteps();

        //Set footsteps based on terrain layer
        if (!onTerrain) return;
        if (TerrainManager.Instance.GetCurrentTerrain() == null || TerrainManager.Instance.GetTerrainData() == null) return;

        int terrainLayerIdx = TerrainManager.Instance.GetTerrainAtPosition(transform.position);
        currentTerrainLayer = TerrainManager.Instance.GetTerrainData().terrainLayers[terrainLayerIdx].name;

        switch (currentTerrainLayer)
        {
            case "TLayer_dirt": currentGround = Ground.GRAVEL;
                break;

            case "NewLayer 2":
                currentGround = Ground.GRAVEL;
                break;

            case "NewLayer 1":
                currentGround = Ground.GRASS;
                break;

            case "NewLayer 17":
                currentGround = Ground.GRASS;
                break;

            case "NewLayer 19":
                currentGround = Ground.GRASS;
                break;

            case "NewLayer 5":
                currentGround = Ground.GRASS;
                break;

            case "T_Corrupted_grass":
                currentGround = Ground.GRASS;
                break;

            case "T_brighter corrupt grass":
                currentGround = Ground.GRASS;
                break;

            case "NewLayer":
                currentGround = Ground.GRASS;
                break;

            case "TLayer_stone":
                currentGround = Ground.STONE;
                break;

            default:
                currentGround = Ground.GRAVEL;
                break;
        }
    }


    private IEnumerator StartFootstepsCoroutine()
    {
        yield return new WaitForSeconds(initialDelay);

        if (isGrounded) SfxManager.Instance.PostEvent(startStep, gameObject);
        startIsPlaying = true;

        while (true)
        {
            startIsPlaying = false;

            yield return new WaitForSeconds(currentFootstepSfxFrequency);

            if (isGrounded) SfxManager.Instance.PostEvent(continousStep, gameObject);
        }
    }
    private void StartFootsteps(InputAction.CallbackContext context)
    {
        if (currentContinousFootsteps != null) return;

        currentContinousFootsteps = StartFootstepsCoroutine();

        StartCoroutine(currentContinousFootsteps);
    }
    private void EndFootsteps(InputAction.CallbackContext context)
    {
        if (currentContinousFootsteps != null) StopCoroutine(currentContinousFootsteps);
        currentContinousFootsteps = null;

        if (startIsPlaying) return;

        if (isGrounded) SfxManager.Instance.PostEvent(endStep, gameObject);
    }


    private void SetSteps() //TO-DO: set this to subscribe to certain events (when we move to another type of terrain, when we go into another body, etc.)
    {
        if(Player.Instance.isOutOfBody)
        {
            //TO-DO: set out of body walking sfx - type of enemy, etc.

            if(Player.Instance.GetCurrentBody() == Player.PlayerBody.Thornshell)
            {
                startStep = "Play_ThornshellMovement";
                continousStep = "Play_ThornshellMovement";
                endStep = "Stop_ThornshellMovement2";
                currentFootstepSfxFrequency = thornshellFootstepSfxFrequency;
            }
            else
            {
                startStep = "";
                continousStep = "";
                endStep = "";
                currentFootstepSfxFrequency = devourerFootstepSfxFrequency;
            }

            return;
        }

        currentFootstepSfxFrequency = playerFootstepsSfxFrequency;

        switch (currentGround)
        {
            case Ground.GRASS:
                startStep = "Player_FootstepsStartGrass_Play";
                continousStep = "Player_FootstepsContinousGrass_Play";
                endStep = "Player_FootstepsEndGrass_Play";
                break;

            case Ground.GRAVEL:
                startStep = "Player_FootstepsStartGravel_Play";
                continousStep = "Player_FootstepsContinousGravel_Play";
                endStep = "Player_FootstepsEndGravel_Play";
                break;

            case Ground.STONE:
                startStep = "Player_FootstepsStartStone_Play";
                continousStep = "Player_FootstepsContinousStone_Play";
                endStep = "Player_FootstepsEndStone_Play";
                break;
        }
    }
}
