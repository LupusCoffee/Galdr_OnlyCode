using UnityEngine;

public class RotateTextTowardPlayer : MonoBehaviour
{
    Transform target;

    Vector3 pivot;

    public void SetPivot(Vector3 pivot)
    {
        this.pivot = pivot + new Vector3(0, 2.5f, 0);
    }

    void Start()
    {
        if(FirstPersonController.Instance != null)
            target = FirstPersonController.Instance.transform;
        else
            target = Player.Instance.transform;
    }

    void Update()
    {

        //face the player at a distance of 1 from the pivot, rotating AROUND the pivot in an orbital fashion
        Vector3 direction = (target.position - pivot).normalized;
        transform.position = pivot + direction;
        transform.LookAt(target);
        transform.Rotate(0, 180, 0);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
}
