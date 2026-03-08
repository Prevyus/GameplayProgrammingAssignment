using System.Collections.Generic;
using UnityEngine;

public class MainMenuPlayer : MonoBehaviour
{// CONTROLS THE PLAYER MODEL THAT IS PRESSING THE PS4 CONTROLLER BUTTONS (ALL AESTHETIC)
    [SerializeField] Ps4Controller ctrl;

    [Header("Settings")]
    public float handSpeed = 20;
    public float middlePointSpeed = 20;

    [Header("Refferences")]
    public Camera cam;
    public Transform RightHand;
    public Transform LeftHand;
    public Transform handsMiddlePoint;
    public Transform BodyMiddlePoint;
    public float bodyToMiddlePointPercent = 0.5f;
    [SerializeField] List<Renderer> BoxMeshRenderers = new List<Renderer>();
    [SerializeField] List<Color> colors = new List<Color>();
    private Color MeshesColor;

    [Header("Buttons")]
    [SerializeField] Transform triangle;
    [SerializeField] Transform square;
    [SerializeField] Transform cross;
    [SerializeField] Transform circle;
    [SerializeField] Transform Up;
    [SerializeField] Transform Left;
    [SerializeField] Transform Down;
    [SerializeField] Transform Right;
    [SerializeField] Transform LeftJoystick;
    [SerializeField] Transform RightJoystick;

    private void Start()
    {
        ApplyColors();
    }

    private void Update()
    {
        CameraMovement();

        MoveHands();
    }

    void CameraMovement()
    {
        // EMPTY
    }

    void MoveHands()
    { // MOVES THE HANDS OF THE CHARACTER TO THE LAST TOUCHED BUTTON
        Transform rightHandTarget = null;
        switch (ctrl.lastRightHandPos)
        {
            case "triangle":
                rightHandTarget = triangle;
                break;
            case "square":
                rightHandTarget = square;
                break;
            case "cross":
                rightHandTarget = cross;
                break;
            case "circle":
                rightHandTarget = circle;
                break;
            case "Right Joystick":
                rightHandTarget = RightJoystick;
                break;
        }
        if (rightHandTarget)
        {
            RightHand.position = Vector3.Lerp(RightHand.position, rightHandTarget.position, Time.deltaTime * handSpeed);
            RightHand.rotation = Quaternion.Slerp(RightHand.rotation, rightHandTarget.rotation, Time.deltaTime * handSpeed);
        }

        Transform leftHandTarget = null;
        switch (ctrl.lastLeftHandPos)
        {
            case "Up":
                leftHandTarget = Up;
                break;
            case "Left":
                leftHandTarget = Left;
                break;
            case "Down":
                leftHandTarget = Down;
                break;
            case "Right":
                leftHandTarget = Right;
                break;
            case "Left Joystick":
                leftHandTarget = LeftJoystick;
                break;
        }
        if (leftHandTarget)
        {
            LeftHand.position = Vector3.Lerp(LeftHand.position, leftHandTarget.position, Time.deltaTime * handSpeed);
            LeftHand.rotation = Quaternion.Slerp(LeftHand.rotation, leftHandTarget.rotation, Time.deltaTime * handSpeed);
        }

        // GETS THE MIDDLE POINT OF BETWEEN THE TWO HANDS TO DETERMINE THE MIDDLE POINT WHICH MOVES THE BODY A LITTLE BIT
        Vector3 target = Vector3.Lerp(LeftHand.position, RightHand.position, 0.5f);
        handsMiddlePoint.position = Vector3.Lerp(handsMiddlePoint.position, target, Time.deltaTime * middlePointSpeed);
        BodyMiddlePoint.localPosition = Vector3.Lerp(Vector3.zero, handsMiddlePoint.localPosition, bodyToMiddlePointPercent);
    }

    void ApplyColors()
    {// ADDS A RANDOM COLOR TO THE CHARACTER
        if (colors.Count > 0)
        {
            int colorNum = Random.Range(0, colors.Count);
            Color randColor = colors[colorNum];
            MeshesColor = randColor;
        }

        foreach (Renderer mesh in BoxMeshRenderers)
        {
            if (mesh) mesh.material.color = MeshesColor;
        }
    }
}
