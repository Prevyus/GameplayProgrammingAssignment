using System.Net;
using UnityEngine;

public class RotateTowards : MonoBehaviour
{// COMPONENT THAT MAKES THIS OBJECT ROTATE TOWARDS THE POSITION OF THE TARGET
    public Transform target;
    public float smoothSpeed = 99999f;

    private void Update()
    {
        Quaternion targetRot = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * smoothSpeed);
    }

}
