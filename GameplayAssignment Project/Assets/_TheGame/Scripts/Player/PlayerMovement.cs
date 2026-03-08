using Toolbox;
using UnityEngine;

public class PlayerMovement : CharacterMovement
{// PLAYER COMPONENT THAT INHERITS FROM THE BASE CHARACTER MOVEMENT, THIS ADDS PLAYER LOOKING MECHANIC WITH MOUSE OR GAMEPAD INPUT
    [HideInInspector] public float headXRotation = 0;
    [Header("Player Specific")]
    [SerializeField] float horizontalLookMinMax = 45;
    [SerializeField] float rollAngleMult = 5;
    [SerializeField] float rollSmoothSpeed = 5;
    
    float yaw;
    float roll; 
    float targetYRot = 0;
    float i = 0;

    public override void UpdateValues()
    {
        base.UpdateValues();

        headXRotation = CameraAnchor.localEulerAngles.x;
    }
    
    public override void LookMovement()
    { // CAMERA MOVEMENT, DECIDED WHETHER TO MOVE ONLY THE HEAD OR WHOLE BODY
      // AND ALSO REALIGNS THE HEAD WITH THE BODY WHEN THE PLAYER STARTS WALKING

        if (!isAlive) { DeadLookMovement(); return; }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        float pitchDelta = MouseVerticalInput * mouseSensitivity * (invertY ? 1f : -1f);
        pitch = Mathf.Clamp(pitch + pitchDelta, minPitch, maxPitch);

        float yawDelta = MouseHorizontalInput * mouseSensitivity;

        roll = Mathf.Lerp(roll, yawDelta * rollAngleMult, Time.deltaTime * rollSmoothSpeed);

        if (!CameraAnchor) return;
        if (movement == 0)
        {
            i = 0;

            targetYRot = transform.eulerAngles.y + yaw;
            if (targetYRot > 360) targetYRot -= 360;
            if (targetYRot < 0) targetYRot += 360;

            yaw = Mathf.Clamp(yaw + yawDelta, -horizontalLookMinMax, horizontalLookMinMax);

            if (Mathf.Abs(yaw) == horizontalLookMinMax && Mathf.Abs(yawDelta) > 0) transform.Rotate(0f, yawDelta, 0f);
        }
        else
        {
            if (i < 1)
            {
                i = Mathf.Clamp(i + (Time.deltaTime * .5f), 0, 1);
                yaw = Mathf.Lerp(yaw, 0, i);
                targetYRot += yawDelta;
                Vector3 v1 = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + yawDelta, transform.eulerAngles.z);
                Vector3 v2 = new Vector3(transform.eulerAngles.x, targetYRot, transform.eulerAngles.z);
                Vector3 lerp = Vector3.Lerp( v1, Mathf.Abs(v1.y - v2.y) < 180 ? v2 : new Vector3(v2.x, v2.y + (360 * Mathf.Clamp(v1.y - v2.y, -1, 1)), v2.z), i);
                transform.eulerAngles = new Vector3(lerp.x, lerp.y , lerp.z);
            }
            else
            {
                transform.Rotate(0f, yawDelta, 0f);
            }
        }
        Quaternion camRot = Quaternion.Euler(pitch, yaw, roll);

        CameraAnchor.localRotation = camRot;
    }

    public override void DeadLookMovement()
    { // CAMERA MOVEMENT FOR WHEN THE PLAYER IS DEAD, ONLY MOVES THE CHARACTERS HEAD
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        float pitchDelta = MouseVerticalInput * mouseSensitivity * (invertY ? 1f : -1f);
        pitch = Mathf.Clamp(pitch + pitchDelta, minPitch, maxPitch);

        float yawDelta = MouseHorizontalInput * mouseSensitivity;

        roll = Mathf.Lerp(roll, yawDelta * rollAngleMult, Time.deltaTime * rollSmoothSpeed);

        if (!CameraAnchor) return;

        i = 0;

        targetYRot = transform.eulerAngles.y + yaw;
        if (targetYRot > 360) targetYRot -= 360;
        if (targetYRot < 0) targetYRot += 360;

        yaw = Mathf.Clamp(yaw + yawDelta, -horizontalLookMinMax, horizontalLookMinMax);

        Quaternion camRot = Quaternion.Euler(pitch, yaw, roll);

        CameraAnchor.localRotation = camRot;
    }
}
