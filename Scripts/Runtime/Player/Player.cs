using System.Threading.Tasks;
using SaveSystem;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static CompactMath;
using System.Collections;
public class Player : MonoBehaviour
{
    public static Player Instance;

    public enum PlayerState { Idle, Walking, /*PlayingMusic,*/ Crouching, Rooted }
    public PlayerState playerState = PlayerState.Idle;
    public enum PlayerBody { Devourer, Thornshell };
    private PlayerBody currentBodyEnum;

    [Header("Movement")]
    bool isMoving;
    PlayerController movement;

    [Header("Cameras & Input")]
    PlayerInput playerInput;
    CinemachineCamera vcam;
    CinemachineOrbitalFollow vcamFollow;

    float horizontalAxisBeforePause = 0;
    float verticalAxisBeforePause = 0;

    [Header("Music / Spell")]
    [SerializeField] GameObject spellParticles;
    [SerializeField] MeshRenderer fluteRenderer;
    public bool isPlayingMusic = false;

    [Header("Ground Check (SphereCast)")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayerMask;
    private bool isGrounded;

    // Animator parameters
    private const string PARAM_PLAYERSTATE = "PlayerState";
    private const string PARAM_IS_AIRBORNE = "IsAirborne";
    private const string PARAM_IS_FLUTEACTIVE = "IsFluteActive";
    // Landing trigger
    private const string TRIGGER_LAND = "IsLanding";

    // Track if we were grounded previously, so we can detect landing
    private bool wasGrounded = true;

    // Landing layer settings
    [Header("Landing Layer Settings")]
    [Tooltip("Index of the landing animation layer in the Animator.")]
    [SerializeField] private int landingLayerIndex = 1;
    [Tooltip("How long the landing layer remains active (seconds).")]
    [SerializeField] private float landingLayerTime = 0.75f;

    public Rigidbody rb { get; private set; }

    public bool isOutOfBody { get; private set; }
    public GameObject currentPlayerBody { get; private set; }
    public int LanguageLevel { get; set; } = 0;

    [SerializeField] bool USE_SPAWNPOINTS = true;

    Animator animator;
    SavedTransform savedTransform;

    private void Awake()
    {
        Instance = this;

        if (playerState == PlayerState.Walking /*|| playerState == PlayerState.Crouching*/)
            this.isMoving = true;
        else
            this.isMoving = false;

        LeanTween.init(800);

        movement = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
        vcam = GetComponentInChildren<CinemachineCamera>();
        vcamFollow = vcam.GetComponent<CinemachineOrbitalFollow>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        savedTransform = GetComponent<SavedTransform>();

        currentPlayerBody = gameObject;

        SpellTargetFinder.Init();
    }

    private void Start()
    {
        if (TransitionAnimator.Instance != null)
            TransitionAnimator.Instance.SetLevelSpawnZone(transform.position);
        else
            Debug.LogError("TransitionAnimator instance is null. \nPlease add the PRE_TransitionAnimator prefab to the scene.");

        bool hasSavedData = savedTransform.HasSavedData();
        if (USE_SPAWNPOINTS && !hasSavedData)
            SpawnpointManager.TeleportPlayer(SceneManager.GetActiveScene().buildIndex);
        else if (hasSavedData)
        {
        #if UNITY_EDITOR
            if (SaveManager.IsLoadedFromMainMenu)
            {
                if (!savedTransform.LoadTransformFromData())
                {
                    if (USE_SPAWNPOINTS)
                        SpawnpointManager.TeleportPlayer(SceneManager.GetActiveScene().buildIndex);
                }
            }
            else if (USE_SPAWNPOINTS)
            {
                SpawnpointManager.TeleportPlayer(SceneManager.GetActiveScene().buildIndex);
            }
        #else
                if (!savedTransform.LoadTransformFromData())
                {
                    if (USE_SPAWNPOINTS)
                        SpawnpointManager.TeleportPlayer(SceneManager.GetActiveScene().buildIndex);
                }
        #endif
        }

        // Check if the player is grounded initially
        CheckGrounded();
        wasGrounded = isGrounded;
    }

    public void ResetPlayerToSave()
    {
        if (!savedTransform.LoadTransformFromData())
        {
            SpawnpointManager.TeleportPlayer(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void Update()
    {
        // 1) Update animation controller with current player state.
        animator.SetInteger(PARAM_PLAYERSTATE, (int)playerState);

        // 2) Check if the player is grounded via SphereCast.
        CheckGrounded();

        // -- Update the IsAirborne bool in the animator --
        animator.SetBool(PARAM_IS_AIRBORNE, !isGrounded);

        // -- Detect landing (transition from airborne to grounded) --
        if (!wasGrounded && isGrounded)
        {
            // Trigger the Land animation
            animator.SetTrigger(TRIGGER_LAND);

            // Also kick off the layer weight routine if you want a dedicated layer for the landing anim
            StartCoroutine(LandingLayerRoutine());
        }
        wasGrounded = isGrounded;

        // 3) Show or hide flute based on isPlayingMusic.
        fluteRenderer.enabled = isPlayingMusic;

        // 4) If the player is rooted or out of body, do not process normal movement logic.
        if (playerState == PlayerState.Rooted || isOutOfBody) {
            animator.SetInteger(PARAM_PLAYERSTATE, 0);
            return;
        }

        // 5) Movement check:
        this.isMoving = UserInputs.Instance.playerMove != Vector2.zero;

        // 6) Decide player state.
        if (this.isMoving /* && isGrounded */)
        {
            playerState = PlayerState.Walking;
        }
        else
        {
            playerState = PlayerState.Idle;
        }
    }

    private void CheckGrounded()
    {
        Vector3 origin = transform.position;
        isGrounded = Physics.SphereCast(
            origin,
            groundCheckRadius,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            groundLayerMask
        );
    }

    /// <summary>
    /// Coroutine that sets the landing layer weight to 1, waits, then sets it back to 0.
    /// </summary>
    private IEnumerator LandingLayerRoutine()
    {
        // Set landing layer weight to 1
        animator.SetLayerWeight(landingLayerIndex, 1f);

        // Wait the specified time
        yield return new WaitForSeconds(landingLayerTime);

        // Return weight back to 0
        animator.SetLayerWeight(landingLayerIndex, 0f);
    }

    private void OnDrawGizmos()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + (Vector3.down * groundCheckDistance);

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(startPos, endPos);
        Gizmos.DrawWireSphere(startPos, groundCheckRadius);
        Gizmos.DrawWireSphere(endPos, groundCheckRadius);
    }

    public void SetGrabbed(bool rooted = true)
    {
        movement.isRooted = rooted;
        if (rooted)
        {
            playerState = PlayerState.Rooted;
        }
        else
        {
            playerState = PlayerState.Idle;
        }
    }

    public void FullyEnablePlayerControls()
    {
        vcam.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        vcamFollow.HorizontalAxis.Value = horizontalAxisBeforePause;
        vcamFollow.VerticalAxis.Value = verticalAxisBeforePause;
    }

    public void FullyDisablePlayerControls()
    {
        vcam.enabled = false;

        Cursor.lockState = CursorLockMode.None;

        horizontalAxisBeforePause = vcamFollow.HorizontalAxis.Value;
        verticalAxisBeforePause = vcamFollow.VerticalAxis.Value;
    }

    public PlayerController GetController() => movement;

    public void EnableOrderedButtonInteraction() { this.movement.EnableOrderedButtonInteraction(); }

    public void DisableOrderedButtonInteraction() { this.movement.DisableOrderedButtonInteraction(); }

    public void ActivatedOneOrderedButton() { this.movement.ActivatedOneOrderedButton(); }

    public void ActivatedClickButton() { this.movement.ActivatedClickButton(); }

    /// <summary>
    /// Sets whether the player is currently playing music on the flute.
    /// Also sets the IsFluteActive bool in the animator.
    /// </summary>
    public void SetIsPlayingMusic(bool value)
    {
        isPlayingMusic = value;
        animator.SetBool(PARAM_IS_FLUTEACTIVE, isPlayingMusic);
        fluteRenderer.enabled = isPlayingMusic;
    }

    public void DisableControls(GameObject newTarget)
    {
        playerState = PlayerState.Rooted;
        isOutOfBody = true;

        //Sfx stuff
        SfxManager.Instance.SetMasterLowpassFull();

        if (newTarget == null)
            return;

        vcam.Target.TrackingTarget = newTarget.transform;
        currentPlayerBody = newTarget;

        Grabber grabber;
        if(newTarget.TryGetComponent<Grabber>(out grabber))
            currentBodyEnum = PlayerBody.Devourer;
        else
            currentBodyEnum = PlayerBody.Thornshell;

        UserInputs.Instance.OnMindControlStart();
    }

    public void EnableControls()
    {
        playerState = PlayerState.Idle;
        vcam.Target.TrackingTarget = transform;
        isOutOfBody = false;

        //Sfx stuff
        SfxManager.Instance.SetMasterLowpassNone();

        currentPlayerBody = gameObject;

        UserInputs.Instance.OnMindControlEnd();
        DisableNoteSheet();
    }

    async void DisableNoteSheet()
    {
        await Task.Delay(1);
        //spellMaker.CloseNoteSheet();
    }

    public void PlaySpellParticles()
    {
        GameObject temp = Instantiate(spellParticles,
            new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z),
            Quaternion.identity);
        temp.transform.SetParent(transform, true);
    }

    public PlayerBody GetCurrentBody() => currentBodyEnum;
}
