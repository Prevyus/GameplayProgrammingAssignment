using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ActivatableObject
{
    public GameObject activatableObject;
    public bool active;
}

public class SetActiveOnStart : MonoBehaviour
{ // TESTING HELPER THAT SETS ITEMS ACTIVE ON START
    [SerializeField] List<ActivatableObject> activatableObjects = new List<ActivatableObject>();
    private void Start()
    {
        foreach (ActivatableObject obj in activatableObjects)
        {
            obj.activatableObject.SetActive(obj.active);
        }
    }
}
