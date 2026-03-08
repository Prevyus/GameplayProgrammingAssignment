using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public struct parentPair
{
    public Transform obj;
    public Transform parent;
}

// THIS CLASS IS JUST TO AUTOMATICALLY PARENT ALL BOX COLLIDERS OF THE CHARACTERS INTO THE CORRECT RIG BONES

public class AutoRigParenting : MonoBehaviour
{
    public List<parentPair> transforms = new List<parentPair>();

    void Awake()
    {
        foreach (parentPair pair in transforms)
        {
            if (pair.obj && pair.parent) pair.obj.parent = pair.parent;
        }
    }
}
