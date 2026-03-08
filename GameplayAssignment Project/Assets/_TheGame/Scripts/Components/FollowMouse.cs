using UnityEngine;

public class FollowMouse : MonoBehaviour
{// MAKES THIS OBJECT MOVE TOWARDS WHERE THE MOUSE IS ON THE SCREEN
    [SerializeField] Camera cam;
    [SerializeField] float distanceFromCamera = 10f;
    [SerializeField] float lerpSpeed = 10f;

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = distanceFromCamera;
        Vector3 worldPosition = cam.ScreenToWorldPoint(mousePosition);
        transform.position = Vector3.Lerp(transform.position, worldPosition, Time.deltaTime * lerpSpeed);
    }
}