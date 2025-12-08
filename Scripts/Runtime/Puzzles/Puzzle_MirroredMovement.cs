using UnityEngine;
using static CompactMath;

public class Puzzle_MirroredMovement : PausableMonoBehaviour
{
    public bool isMindControlled = false;
    Vector2 moveInput;
    PlayerController playerController;

    private void Start()
    {
        playerController = Player.Instance.GetComponent<PlayerController>();
    }

    public void ActivateMindControl()
    {
        isMindControlled = true;
    }


    private void Update()
    {
        if (isMindControlled && !IsPaused && !IsApproximately(playerController.Velocity, 0, 0.01f))
        {
            Debug.LogError("COMMENTED OUT STUFF HERE. It's giving me errors.");

            Quaternion targetRotation = playerController.transform.rotation;
            transform.rotation = targetRotation;
            transform.position += transform.forward * playerController.MovementSpeed * Time.deltaTime;
        }
    }

    public bool IsPlayerMoving()
    {
        return playerController.Velocity > 0.1F;
    }
}
