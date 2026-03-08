using UnityEngine;
using System;

public class CollisionDetector : MonoBehaviour
{// THIS IS A COMPONENT YOU CAN PUT NEXT TO A COLLIDER IF YOU HAVE AN OBJECT THAT HAD VARIOUS COLLIDERS AS CHILDREN OF IT
    public bool isColliding;

    public event Action<float> OnHit;
    public void Hit(float amount)
    {
        OnHit?.Invoke(amount);
    }

    #region Collider
    [HideInInspector] public Collider Other;
    [HideInInspector] public Collision Collision;

    public event Action onTriggerEnter;
    public event Action onTriggerStay;
    public event Action onTriggerExit;

    public event Action onCollisionEnter;
    public event Action onCollisionStay;
    public event Action onCollisionExit;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"{other.gameObject.name} enter");
        onTriggerEnter?.Invoke();
        isColliding = true;
        Other = other;
    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log($"{other.gameObject.name} stay");
        onTriggerStay?.Invoke();
        isColliding = true;
        Other = other;
    }

    private void OnTriggerExit(Collider other)
    {   
        //Debug.Log($"{other.gameObject.name} exit");
        onTriggerExit?.Invoke();
        isColliding = false;
        Other = null;
    }

    private void OnCollisionEnter(Collision collision)
    {        
        //Debug.Log($"{other.gameObject.name} enter");
        onCollisionEnter?.Invoke();
        isColliding = true;
        Collision = collision;
    }

    private void OnCollisionStay(Collision collision)
    {        
        //Debug.Log($"{other.gameObject.name} stay");
        onCollisionStay?.Invoke();
        isColliding = true;
        Collision = collision;
    }

    private void OnCollisionExit(Collision collision)
    {
        //Debug.Log($"{other.gameObject.name} exit");
        onCollisionExit?.Invoke();
        isColliding = false;
        Collision = null;
    }
    #endregion
}
