using Custom;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicalButton : MonoBehaviour, IInteractable
{
    [SerializeField] float speed = 10;
    [SerializeField] bool pressOnStart = false;
    Transform buttonObj;
    Vector3 originalPos;
    Quaternion originalRot;
    float targetLerp = 0;

    public virtual void Start()
    {
        buttonObj = transform.GetChild(0);
        originalPos = buttonObj.localPosition;
        originalRot = buttonObj.localRotation;

        if (pressOnStart) Activate(null);
    }

    private void Update()
    {
        ButtonMovement();
    }

    void ButtonMovement()
    {
        if (targetLerp >= 0) targetLerp = Mathf.Lerp(targetLerp, 0, Time.deltaTime * speed);

        Vector3 targetPos = Vector3.Lerp(originalPos, Vector3.zero, targetLerp);
        buttonObj.localPosition = Vector3.Lerp(buttonObj.localPosition, targetPos, Time.deltaTime * speed);

        Quaternion targetRot = Quaternion.Slerp(originalRot, Quaternion.identity, targetLerp);
        buttonObj.localRotation = Quaternion.Slerp(buttonObj.localRotation, targetRot, Time.deltaTime * speed);
    }

    public virtual void Activate(IInteractor interactor)
    {
        targetLerp = 1;
    }
}
