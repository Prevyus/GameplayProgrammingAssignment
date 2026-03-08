using System;
using UnityEngine;
using Toolbox;
using Custom;

public class PlayerInteraction : MonoBehaviour, IInteractor
{// COMPONENT IN PLAYER THAT CONTROLS THE INTERACTIONS WITH INTERACTABLE OBJECTS
    bool isAlive = true;

    [HideInInspector] public PlayerController playerController;

    [SerializeField] float lookingRange = 1.75f;
    [SerializeField] GameObject toggleObject;
    [SerializeField] Transform lookOrigin;

    public Action onInteract;
    bool isInteracting = false;
    [HideInInspector] public IInteractable lastInteraction;

    public void Death()
    {
        isAlive = false;
    }


    private void Start()
    {
        if (lookOrigin == null) lookOrigin = playerController.playerCamera.transform;
    }

    private void Update()
    {
        if (!isAlive) return;

        SearchForInteraction();
    }

    void SearchForInteraction()
    {// TURNS ON OR OFF THE INTERACTION TEXT AND ALLOWS THE INTERACTABLE OBJECTS TO BE ACTIVATED

        IInteractable hitInterface = null;
        GameObject hitGameObject = null;
        bool hitInteractable = tb.CheckForComponentAndTagByRaycast<IInteractable>("Item", lookOrigin.position, lookOrigin.forward, lookingRange, out hitInterface, out hitGameObject, false);
        //Debug.DrawLine(lookOrigin.position, lookOrigin.forward*10, Color.red, 0.1f);
        bool interacting = isInteracting;
        isInteracting = false;
        if (hitInteractable)
        {
            if (toggleObject/* && hitInterface != lastInteraction*/) toggleObject.SetActive(true);

            if (interacting)
            {
                Interact(hitInterface);
            }
        }
        else
        {
            if (toggleObject) toggleObject.SetActive(false);
        }
    }

    public void HandleInteractEvent()
    {
        isInteracting = true;
    }

    public void Interact(IInteractable interactable)
    {// ACTUALLY INTERACTS WITH THE INTERACTABLE OBJECT
        lastInteraction = interactable;
        onInteract?.Invoke();
        interactable.Activate(this);
    }
}
