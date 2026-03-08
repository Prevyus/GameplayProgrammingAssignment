using UnityEngine;

public class FollowPosition : MonoBehaviour
{ // THIS JUST MAKES THIS OBJECT FOLLOW THE POSITION OF THE TARGET
    public Transform target;
    public bool instant;
    public float smoothSpeed = 10;

    void Update()
    {
        if (instant) transform.position = target.position;
        else transform.position = Vector3.Lerp(transform.position, target.position, smoothSpeed * Time.deltaTime);
    }
}
