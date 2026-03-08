using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;

public class CheckIfUnderground : MonoBehaviour
{// THIS IS JUST A QUICK COMPONENT THAT BRINGS OBJECTS BACK TO THE SURFACE IF THEY HAVE ACCIDENTALLY GLITCHED UNDER THE GROUND
    [SerializeField] LayerMask layer;
    [SerializeField] int checkDistance = 10;
    [SerializeField] float teleportAmount = 0.1f;

    void Update()
    {
        Collider hit = null;
        bool isUnderground = tb.CheckForLayerByRaycast
        (
            layerMask: layer, 
            origin: transform.position, 
            direction: Vector3.up,
            maxDistance: checkDistance,
            hit: out hit,
            false
        );
        if (isUnderground) transform.position += new Vector3(0, teleportAmount, 0);
    }
}
