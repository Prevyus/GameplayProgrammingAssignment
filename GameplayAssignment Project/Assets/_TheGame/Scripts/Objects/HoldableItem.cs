using System.Collections.Generic;
using UnityEngine;
using Custom;

[System.Serializable]
public struct ArmSet
{// THIS STRUCT STORES TRANSFORMS OF WHERE THE ARMS SHOULD MOVE WHEN HOLDING THIS ITEM, ALSO THE SETTINGS OF THE MOVEMENT OF THIS ARMSET

    public string armSetName;

    [Header("Refferences")]
    public Transform set;
    public Transform camPos;
    public Transform item;
    public Transform rightElbow;
    public Transform rightHand;
    public Transform leftElbow;
    public Transform leftHand;

    [Header("Movement")]
    public bool breathing;
    public float breathingAmplitude;
    public float breathingFrequency;
    public bool swaying;
    public float swayIntensity;

    [Header("Settings")]
    public bool moveCamera;
    public bool followCamera;
}

public class HoldableItem : MonoBehaviour
{ // THIS CLASS HOLDS ALL DIFFERENT ARM POSITIONS FOR THIS ITEM
    public PlayerItemHolder playerItemHolder;

    [Header("Hand Positions")]
    public Transform Item;
    public List<ArmSet> armSets = new List<ArmSet>();
    public ArmSet current;

    private void Awake()
    {
        if (armSets.Count > 0 && armSets[0].rightHand) Item.transform.position = armSets[0].rightHand.position;
        if (armSets.Count > 0) current = armSets[0];
    }
}
