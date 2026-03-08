using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{ // THIS CLASS IS A GENERIC MOVEMENT COMPONENT THAT OTHER CLASSES CAN INHERIT FROM US DIRECTLY USE FOR CHARACTER MOVEMENT
    public bool isAlive = true;

    [Header("Stats")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float crouchedSpeed = 2f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 2f;
    public bool invertY = false;
    public float minPitch = 89f;
    public float maxPitch = -89f;

    [Header("References")]
    public Transform CameraAnchor;
    public Transform Feet;
    [HideInInspector] public Transform EyePosition; 
    [SerializeField] LayerMask groundMask;

    [Header("Values")]
    [HideInInspector] public float movement;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] float LandingCheckOffset = 1f;
    [HideInInspector] public float MouseHorizontalInput = 0;
    [HideInInspector] public float MouseVerticalInput = 0;
    [HideInInspector] public float HorizontalInput = 0;
    [HideInInspector] public float VerticalInput = 0;
    [HideInInspector] public bool SprintInput = false;
    [HideInInspector] public bool CrouchInput = false;

    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isLanding;
    public event Action OnJump;

    [Header("Stamina")]
    [HideInInspector] public bool isRunning = false;
    [SerializeField] bool UseStamina = false;
    [SerializeField] float MaxStamina = 100;
    float currentStamina = 0;
    [HideInInspector] public float stamina => MaxStamina > 0f ? currentStamina / MaxStamina : 0f;
    [SerializeField] float staminaRegenMult = 1;
    [SerializeField] float staminaRegenDelayTime = .5f;
    float staminaRegenDelayTimer;

    CharacterController Controller;
    Vector3 velocity;
    public float pitch;

    public virtual void Death()
    {
        isAlive = false;
    }

    private void Awake()
    {
        if (Controller == null) Controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        UpdateValues();
    }

    public virtual void UpdateValues()
    {
        CameraAnchor.position = EyePosition.position;
        isGrounded = CheckIfGrounded(false, 0);
        isLanding = CheckIfGrounded(true, LandingCheckOffset * Mathf.Max(Mathf.Abs(Controller.velocity.y), 0.1f));
    }

    bool CheckIfGrounded(bool useRaycast, float offset)
    {
        if (useRaycast) return Physics.Raycast(Feet.position, Vector3.down, offset, groundMask, QueryTriggerInteraction.Ignore);

        else return Physics.CheckSphere(new Vector3(Feet.position.x, Feet.position.y - offset, Feet.position.z), groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
    }

    public virtual void LookMovement()
    {
        if (!isAlive) { DeadLookMovement(); return; }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        float yawDelta = MouseHorizontalInput * mouseSensitivity;
        float pitchDelta = MouseVerticalInput * mouseSensitivity * (invertY ? 1f : -1f);
        pitch = Mathf.Clamp(pitch + pitchDelta, minPitch, maxPitch);
        transform.Rotate(0f, yawDelta, 0f);
        if (CameraAnchor)
        {
            Quaternion camRot = Quaternion.Euler(pitch, 0f, 0f);
            CameraAnchor.localRotation = camRot;
        }
    }
    public virtual void DeadLookMovement()
    {
        // THIS IS MOVEMENT FOR WHEN THIS CHARACTER IS DEAD, CURRENTLY ONLY DOES ANYTHING ON THE PLAYER
    }

    public virtual void WalkMovement()
    {// THIS MOVEMENT TAKES STAMINA, CROUCHING, SPRINTING AND WALKING DIRECTION INTO ACCOUNT
        if (!isAlive) return;

        if (isGrounded && velocity.y < 0) velocity.y = -2f;
        
        float speed = SprintInput && HorizontalInput == 0 && VerticalInput > 0 && currentStamina > 0 ? sprintSpeed : walkSpeed;
        speed = CrouchInput ? crouchedSpeed : speed;
        Vector3 move = new Vector3(HorizontalInput, 0f, VerticalInput).normalized;
        if (CameraAnchor)
        {
            Vector3 forward = CameraAnchor.forward;
            Vector3 right = CameraAnchor.right;
            forward.y = 0;
            right.y = 0;
            move = (forward * VerticalInput + right * HorizontalInput).normalized;
        }
        movement = move.magnitude;
        if (Controller && Controller.enabled) Controller.Move(move * speed * Time.deltaTime);

        if (SprintInput && HorizontalInput == 0 && VerticalInput > 0) staminaRegenDelayTimer = staminaRegenDelayTime;
        if (speed == sprintSpeed && move.magnitude > 0 && UseStamina)
        {
            if (currentStamina > 0) currentStamina -= Time.deltaTime;
            else currentStamina = 0;
        }
        else if (staminaRegenDelayTimer <= 0)
        {
            if (currentStamina < MaxStamina) currentStamina += Time.deltaTime * staminaRegenMult;
            else currentStamina = MaxStamina;
        }
        else staminaRegenDelayTimer -= Time.deltaTime;

        isRunning = speed == sprintSpeed;
        velocity.y += gravity * Time.deltaTime;
        if (Controller && Controller.enabled) Controller.Move(velocity * Time.deltaTime);
    }

    public virtual void Jump()
    {
        if (!isAlive) return;

        if (isGrounded)
        {
            OnJump?.Invoke();
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (Controller && Controller.enabled) Controller.Move(velocity * Time.deltaTime);
        }
    }
}
