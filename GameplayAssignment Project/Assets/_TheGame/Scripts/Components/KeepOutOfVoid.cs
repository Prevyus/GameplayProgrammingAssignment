using UnityEngine;

public class KeepOutOfVoid : MonoBehaviour
{// COMPONENT THAT MAKES THE OBJECT TELEPORT BACK UP IF IT GOES TO FAR INTO THE VOID
    [SerializeField] float teleportHeight = 10f;
    [SerializeField] float minHeight = -10f;
    [SerializeField] bool returnToZero;
    [SerializeField] float range = 100;
    void Update()
    {
        if (transform.position.y < minHeight)
        {
            Vector3 tp = new Vector3(transform.position.x, teleportHeight, transform.position.z);
            transform.position = tp;
        }
        if (!returnToZero) return;

        float distance = (Vector3.zero - new Vector3(transform.position.x, 0, transform.position.z)).magnitude;
        if (Mathf.Abs(distance) > range)
        {
            transform.position = new Vector3(0, transform.position.y, 0);
        }
    }
}
