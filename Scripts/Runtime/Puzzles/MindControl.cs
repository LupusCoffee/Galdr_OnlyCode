using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static CompactMath;
using static Player;

public class MindControl : PausableMonoBehaviour
{
    public bool isMindControlled = false; //used as long as the object is being mind controlled
    public bool hasBeenPossessed = false; //used only temporarily as a flag when activating
    public bool hasBeenUnpossessed = false; //used only temporarily as a flag when deactivating
    Vector2 moveInput;


    Rigidbody rb;

    [SerializeField] LayerMask groundMask;
    [SerializeField] float moveSpeed;
    [SerializeField] bool canChangeYLevel = true;

    public PlayerState playerState = PlayerState.Idle;

    bool isMoving;

    float gracePeriod = 2;
    float gracePeriodTimer = 0;

    Enemy enemy;

    private bool canMove = true;

    [SerializeField] private AK.Wwise.Event oneTimeEventOnInteraction;
    bool oneTimeEventHappened = false;

    [Header("Thornshell related")]
    [SerializeField] Animator animator;
    [SerializeField] bool isThornShell = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemy = GetComponentInChildren<Enemy>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        UserInputs.Instance._activateNoteSheet.performed += OnActivateNoteSheet;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        UserInputs.Instance._activateNoteSheet.performed -= OnActivateNoteSheet;
    }

    public void ActivateMindControl() {
        isMindControlled = true;
        hasBeenPossessed = true;
        gracePeriodTimer = gracePeriod;

        PlayOneTimeEvent(oneTimeEventOnInteraction, gameObject);
        OverlayHandler.Instance.ShowOverlayMindControl();

        Grabber grabber;
        if (gameObject.TryGetComponent<Grabber>(out grabber))
        {
            SfxManager.Instance.PostEvent("TentacleMove");
        }

        if (isThornShell)
            animator.SetBool("HornsActive", true);
    }

    public void DeactivateMindControl() {
        isMindControlled = false;
        hasBeenUnpossessed = true;
        Player.Instance.gameObject.GetComponent<OutlineObject>().SetOutlinePink(false);
        //check if the player has anything in InteractZone?
        InteractPrompt.Instance.HideInteractPrompt();
        OverlayHandler.Instance.HideOverlayMindControl();
        SfxManager.Instance.PostEvent("TentacleStop");

        if (isThornShell)
            animator.SetBool("HornsActive", false);
    }


    private void Update()
    {
        if (gracePeriodTimer > 0) {
            gracePeriodTimer -= Time.deltaTime;
        }

        moveInput = UserInputs.Instance.playerMove;
        if (moveInput != Vector2.zero && isMindControlled && !IsPaused && canMove)
        {
            Vector3 flatForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;

            float horizontalInput = moveInput.x;
            float verticalInput = moveInput.y;

            Vector3 moveDirection = flatForward * verticalInput + Camera.main.transform.right * horizontalInput;

            Vector3 futureCheck = moveDirection.normalized;

            //check raycast down from the future position to see if it will hit the ground, if not, don't move
            RaycastHit hitInfo;
            if (!canChangeYLevel) {
                Debug.DrawRay(transform.position + futureCheck * 1.2f, Vector3.down * 0.75f, Color.yellow);
                if (!Physics.Raycast(transform.position + futureCheck * 1.2f, Vector3.down, out hitInfo, 0.75f, groundMask, QueryTriggerInteraction.Ignore))  {
                    Debug.Log("Can't move there " );
                    return;
                }
                else
                    Debug.Log("hit " + hitInfo.collider.gameObject.name);
            }

            transform.position += moveDirection * moveSpeed * ScaledDeltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, moveSpeed * 100 * ScaledDeltaTime);

            
            
            animator.SetBool("IsMoving", true);
            //Debug.Log("IsMoving true");
            
           
        }
        else 
        {
            if (isThornShell)
            {
                animator.SetBool("IsMoving", false);
                //Debug.Log("IsMoving false");
            }
        }
    }

    public void OnMove()
    {
        moveInput = UserInputs.Instance.playerMove;
    }

    public void OnActivateNoteSheet(InputAction.CallbackContext ctx)
    {
        if (isMindControlled && gracePeriodTimer <= 0)
        {
            DeactivateMindControl();
            Player.Instance.EnableControls();
            enemy.PlayerControl_Deactivate();
        }
    }

    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    private void PlayOneTimeEvent(AK.Wwise.Event oneTimeEvent, GameObject _gameObject)
    {
        if (!oneTimeEventHappened)
        {
            SfxManager.Instance.PostEvent(oneTimeEvent, _gameObject);
            oneTimeEventHappened = true;
        }
    }
}
