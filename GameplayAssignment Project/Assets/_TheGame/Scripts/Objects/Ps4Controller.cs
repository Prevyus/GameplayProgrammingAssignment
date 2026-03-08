using UnityEngine;
using UnityEngine.InputSystem;

public class Ps4Controller : MonoBehaviour
{ // CLASS THAT CONTROLS THE JOYSTICKS AND BUTTONS OF THE PS4 CONTROLLER MODEL
    public PlayerInput input;
    public HoldableItem holdableItem;

    [Header("Settings")]
    [SerializeField] float leftJoystickAngle = 20f;
    [SerializeField] float rightJoystickAngle = 20f;

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

    public string lastRightHandPos;
    public string lastLeftHandPos;

    private void Start()
    {
        holdableItem = GetComponentInParent<HoldableItem>();
        if (holdableItem && !input) input = holdableItem.playerItemHolder.GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (!input) return;

        if (input.usingGamepad) // DIFFERENT INPUTS DEPENDING ON IF THE USER HAS A GAMEPAD CONNECTED OR NOT
        {
            if (!input.CrouchInput && !input.DropInput && !input.InteractInput && !input.ReloadInput && !input.JumpInput && !input.PrimarySelectInput) lastRightHandPos = "Right Joystick";
            if (!input.UpInput && !input.DownInput && !input.LeftInput && !input.RightInput) lastLeftHandPos = "Left Joystick";

            PressButton(triangle, input.InventoryInput);
            PressButton(circle, input.DropInput);
            PressButton(square, input.InteractInput);
            PressButton(cross, input.JumpInput);

            PressDPad(Up, input.UpInput, -15, 0);
            PressDPad(Down, input.DownInput, 15, 0);
            PressDPad(Left, input.LeftInput, 0, 15);
            PressDPad(Right, input.RightInput, 0, -15);

            MoveJoystick(LeftJoystick, input.HorizontalInput * 5 , input.VerticalInput * 5, leftJoystickAngle, false);
            MoveJoystick(RightJoystick, input.LookHorizontalInput * 10,input.LookVerticalInput * 10, rightJoystickAngle, true);
        }
        else
        {
            if (!input.CrouchInput && !input.DropInput && !input.InteractInput && !input.ReloadInput && !input.JumpInput && !input.PrimarySelectInput) lastRightHandPos = RightJoystick.name;
            if (!input.UpInput && !input.DownInput && !input.LeftInput && !input.RightInput) lastLeftHandPos = LeftJoystick.name;

            PressButton(triangle, input.InventoryInput);
            PressButton(circle, input.CrouchInput || input.DropInput);
            PressButton(square, input.InteractInput || input.ReloadInput);
            PressButton(cross, input.JumpInput || input.PrimarySelectInput);

            PressDPad(Up, input.UpInput, -15, 0);
            PressDPad(Down, input.DownInput, 15, 0);
            PressDPad(Left, input.LeftInput, 0, 15);
            PressDPad(Right, input.RightInput, 0, -15);

            MoveJoystick(LeftJoystick, input.HorizontalInput, input.VerticalInput, leftJoystickAngle, false);
            MoveJoystick(RightJoystick, input.LookHorizontalInput, input.LookVerticalInput, rightJoystickAngle, true);
        }
    }

    void MoveJoystick(Transform joystick, float horizontal, float vertical, float mult, bool rightSide)
    {
        float yRotation = Mathf.Clamp(horizontal * mult, -mult, mult);
        float xRotation = Mathf.Clamp(vertical * mult, -mult, mult);
        string lastPos = rightSide ? lastRightHandPos : lastLeftHandPos;
        Quaternion target = lastPos == joystick.name ? Quaternion.Euler(-xRotation, -yRotation, 0) : Quaternion.identity;

        joystick.localRotation = Quaternion.Slerp(joystick.localRotation, target, Time.deltaTime * 10);
    }

    void PressButton(Transform button, bool pressed)
    {
        if (pressed) lastRightHandPos = button.name;

        float targetZ = pressed ? -0.00095f : 0;
        button.localPosition = Vector3.Lerp(button.localPosition, new Vector3(button.localPosition.x, button.localPosition.y, targetZ), Time.deltaTime * 10);
    }

    void PressDPad(Transform button, bool pressed, float Xangle, float Yangle)
    {
        if (pressed) lastLeftHandPos = button.name;

        float targetX = pressed ? Xangle : 0;
        float targetY = pressed ? Yangle : 0;
        Quaternion targetRot = Quaternion.Euler(targetX, targetY, 0);

        button.localRotation = Quaternion.Slerp(button.localRotation, targetRot, Time.deltaTime * 10);
    }
}
