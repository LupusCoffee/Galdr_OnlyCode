using UnityEngine;
using UnityEngine.InputSystem;

public class Possessable : SpellTarget
{
    public bool isMindControlled = false; //used as long as the object is being mind controlled
    public bool hasBeenPossessed = false; //used only temporarily as a flag when activating
    public bool hasBeenUnpossessed = false; //used only temporarily as a flag when deactivating

    protected Vector2 moveInput;

    [SerializeField] protected LayerMask groundMask;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected bool canChangeYLevel = true;

    float gracePeriod = 1;
    float gracePeriodTimer = 0;

    Enemy enemy;

    private bool canMove = true;

    [SerializeField] private AK.Wwise.Event oneTimeEventOnInteraction;

    [Header("Thornshell related")]
    [SerializeField] protected Animator animator;
    [SerializeField] bool isThornShell = false;

    private Vector3 flatForward = Vector3.zero;

    protected virtual void Awake()
    {
        enemy = GetComponentInChildren<Enemy>();
    }

    private void Start()
    {
        moveInput = UserInputs.Instance.playerMove;
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

    public override void SpellHit()
    {
        if (gracePeriodTimer > 0)
        {
            return;
        }

        if (!isMindControlled)
        {
            ActivateMindControl();
            Player.Instance.DisableControls(gameObject);
            enemy.PlayerControl_Activate();
        }
        //else
        //{
        //    DeactivateMindControl();
        //    Player.Instance.EnableControls();
        //    enemy.PlayerControl_Deactivate();
        //}
    }

    public virtual void ActivateMindControl()
    {
        isMindControlled = true;
        hasBeenPossessed = true;
        gracePeriodTimer = gracePeriod;

        //PlayOneTimeEvent(oneTimeEventOnInteraction, gameObject);

        OverlayHandler.Instance.ShowOverlayMindControl();
    }

    public virtual void DeactivateMindControl()
    {
        isMindControlled = false;
        hasBeenUnpossessed = true;
        Player.Instance.gameObject.GetComponent<OutlineObject>().SetOutlinePink(false);

        //check if the player has anything in InteractZone?
        InteractPrompt.Instance.HideInteractPrompt();
        OverlayHandler.Instance.HideOverlayMindControl();
    }

    protected virtual void Update()
    {
        moveInput = UserInputs.Instance.playerMove;

        if (gracePeriodTimer > 0) {
            gracePeriodTimer -= Time.deltaTime;
        }

        if (moveInput == Vector2.zero || !isMindControlled || IsPaused || !canMove)
            return;

        flatForward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;

        float horizontalInput = moveInput.x;
        float verticalInput = moveInput.y;

        Vector3 moveDirection = flatForward * verticalInput + Camera.main.transform.right * horizontalInput;
        Vector3 futureCheck = moveDirection.normalized;

        //check raycast down from the future position to see if it will hit the ground, if not, don't move
        RaycastHit hitInfo;
        if (!canChangeYLevel)
        {
            if (!Physics.Raycast(transform.position + futureCheck * 1.2f, Vector3.down, out hitInfo, 0.75f, groundMask, QueryTriggerInteraction.Ignore))
            {
                return;
            }
        }

        transform.position += moveDirection * moveSpeed * ScaledDeltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, moveSpeed * 100 * ScaledDeltaTime);
    }


    public void OnMove()
    {
        moveInput = UserInputs.Instance.playerMove;
    }

    public void OnActivateNoteSheet(InputAction.CallbackContext ctx)
    {
        if (isMindControlled && gracePeriodTimer <= 0)
        {
            Debug.Log("Deactivate Mind Control");
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
