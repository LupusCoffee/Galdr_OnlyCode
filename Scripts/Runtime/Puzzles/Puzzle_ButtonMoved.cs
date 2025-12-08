using UnityEngine;

public class Puzzle_ButtonMoved : MonoBehaviour
{
    public bool isActivated = false;

    [SerializeField] Puzzle_PressurePlate activateButton;
    [SerializeField] Vector3 moveOffset;
    [SerializeField] float moveTime = 1;

    private Vector3 startPos;
    private float moveSpeedModifier;

    private void Start()
    {
        if (activateButton != null)
        {
            activateButton.OnButtonPressed += ActivateSelf;
            activateButton.OnButtonReleased += DeactivateSelf;
        }

        startPos = transform.position;
        moveSpeedModifier = 1 / moveTime;
    }

    private void Update()
    {
        if (isActivated)
        {
            transform.position = Vector3.Lerp(transform.position, startPos + moveOffset, moveSpeedModifier);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, startPos, moveSpeedModifier);
        }
    }

    public void ActivateSelf(GameObject obj)
    {
        isActivated = true;
    }

    public void DeactivateSelf(GameObject obj)
    {
        isActivated = false;
    }

    private void OnDestroy()
    {
        if (activateButton != null)
        {
            activateButton.OnButtonPressed -= ActivateSelf;
            activateButton.OnButtonReleased -= DeactivateSelf;
        }
    }
}
