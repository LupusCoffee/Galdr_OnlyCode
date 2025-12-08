using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField]
    Transform character;
    public float sensitivity = 2;
    public float smoothing = 1.5f;

    Vector2 velocity;
    Vector2 frameVelocity;

    void Reset()
    {
        character = GetComponentInParent<FirstPersonController>().transform;
    }

    void Start()
    {
        // Lock the mouse cursor to the game screen.
        Cursor.lockState = CursorLockMode.Locked;


        //
        //character.gameObject.transform.rotation = new Quaternion(0, 70, 0, 0);
        Vector3 characterEuler = character.localRotation.eulerAngles;
        Vector3 cameraEuler = transform.localRotation.eulerAngles;

        velocity.x = characterEuler.y;
        velocity.y = -cameraEuler.x; // inverted for pitch
    }

    void Update()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // budget and temporary solution for controller input
        Vector2 controllerDelta = Vector2.zero;
        if (Gamepad.current != null)
        {
            controllerDelta = Gamepad.current.rightStick.ReadValue();
        }


        Debug.Log("controllerDelta:    " + controllerDelta);

        Vector2 combinedDelta = mouseDelta + controllerDelta;

        Vector2 rawFrameVelocity = Vector2.Scale(combinedDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }
}
