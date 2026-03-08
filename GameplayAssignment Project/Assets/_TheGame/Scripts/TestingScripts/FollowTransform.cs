using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform target;
    void Update()
    {
        if (!target) return;
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
