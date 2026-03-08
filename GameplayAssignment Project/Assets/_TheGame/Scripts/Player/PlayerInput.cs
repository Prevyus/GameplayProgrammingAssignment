using UnityEngine;
using System;
using UnityEngine.InputSystem;
using Custom;

public class PlayerInput : MonoBehaviour
{// CLASS THAT RECEIVES ALL THE INPUT FROM THE USER AND STORES IT IN VARIABLES FOR OTHER COMPONENTS TO USE

    bool isAlive = true;

    [SerializeField] bool useNewInputSystem = true;
    [SerializeField] float mouseSensitivity = 4f;
    [SerializeField] float gamepadSensitivity = 20f;
    [SerializeField] float scrollWheelDelay = 0.2f;

    public bool usingGamepad = false;

    [HideInInspector] public float LookHorizontalInput = 0;
    [HideInInspector] public float LookVerticalInput = 0;
    [HideInInspector] public float additionalLookHorizontalInput = 0;
    [HideInInspector] public float additionalLookVerticalInput = 0;
    [HideInInspector] public float HorizontalInput = 0;
    [HideInInspector] public float VerticalInput = 0;
    [HideInInspector] public bool SprintInput = false;
    [HideInInspector] public bool CrouchInput = false;

    [HideInInspector] public bool JumpInput = false;
    [HideInInspector] public bool InventoryInput = false;
    [HideInInspector] public bool ReloadInput = false;
    [HideInInspector] public bool InteractInput = false;
    [HideInInspector] public bool DropInput = false;
    [HideInInspector] public bool PrimarySelectInput = false;
    [HideInInspector] public bool UpInput = false;
    [HideInInspector] public bool DownInput = false;
    [HideInInspector] public bool LeftInput = false;
    [HideInInspector] public bool RightInput = false;

    public event Action OnJump;
    public event Action OnInventory;
    public event Action OnInteract;
    public event Action OnDrop;
    public event Action OnPause;

    public event Action OnPrimaryActionDown;
    public event Action OnSecondaryActionDown;
    public event Action OnTerciaryActionDown;

    public event Action OnPrimaryActionUp;
    public event Action OnSecondaryActionUp;
    public event Action OnTerciaryActionUp;

    [HideInInspector] public bool HoverLootInput = false;

    public event Action OnHotbarUp;
    public event Action OnHotbarDown;
    public event Action<int> OnHotbarButton;
    public event Action OnHolsterItem;

    float scrollDelay = 0;

    InputAction lookAction;
    InputAction moveAction;
    InputAction sprintAction;
    InputAction crouchAction;

    InputAction jumpAction;
    InputAction inventoryAction;
    InputAction reloadAction;
    InputAction interactAction;
    InputAction dropAction;

    InputAction primaryAction;
    InputAction secondaryAction;
    InputAction terciaryAction;

    InputAction hoverLootAction;
    InputAction scrollAction;

    InputAction hotbarUpAction;
    InputAction hotbarDownAction;
    InputAction holsterAction;

    public void Death()
    {
        isAlive = false;
    }

    void Awake()
    {
        SetupNewInputSystemActions();
    }

    void OnEnable()
    {
        EnableNewInputSystemActions(true);
    }

    void OnDisable()
    {
        EnableNewInputSystemActions(false);
    }

    void OnDestroy()
    {
        DisposeNewInputSystemActions();
    }

    private void Start()
    {
        if (GameRoot.Instance) OnPause += GameRoot.Instance.TogglePause;
    }

    private void Update()
    {
        if (useNewInputSystem) // IF THE BOOL FOR NEW INPUT SYSTEM IS CHECKED
        {
            if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
                usingGamepad = true;

            if (Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame)
                usingGamepad = false;

            Vector2 look = lookAction != null ? lookAction.ReadValue<Vector2>() : Vector2.zero;
            Vector2 move = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;

            LookHorizontalInput = (look.x * ((usingGamepad ? gamepadSensitivity : mouseSensitivity) / 100)) + additionalLookHorizontalInput;
            LookVerticalInput = (look.y * ((usingGamepad ? gamepadSensitivity : mouseSensitivity) / 100)) + additionalLookVerticalInput ;
            additionalLookHorizontalInput = 0;
            additionalLookVerticalInput = 0;

            HorizontalInput = move.x;
            VerticalInput = move.y;

            SprintInput = sprintAction != null && sprintAction.IsPressed();
            CrouchInput = crouchAction != null && crouchAction.IsPressed();

            JumpInput = jumpAction != null && jumpAction.IsPressed();
            InventoryInput = inventoryAction != null && inventoryAction.IsPressed();
            ReloadInput = reloadAction != null && reloadAction.IsPressed();
            InteractInput = interactAction != null && interactAction.IsPressed();
            DropInput = dropAction != null && dropAction.IsPressed();

            PrimarySelectInput = primaryAction != null && primaryAction.IsPressed();

            //PauseInput = pauseAction != null && pauseAction.IsPressed();

            bool up = false;
            bool down = false;
            bool left = false;
            bool right = false;

            var kb = Keyboard.current;
            if (kb != null)
            {
                up |= kb.upArrowKey.isPressed;
                down |= kb.downArrowKey.isPressed;
                left |= kb.leftArrowKey.isPressed;
                right |= kb.rightArrowKey.isPressed;
            }

            var gp = Gamepad.current;
            if (gp != null)
            {
                up |= gp.dpad.up.isPressed;
                down |= gp.dpad.down.isPressed;
                left |= gp.dpad.left.isPressed;
                right |= gp.dpad.right.isPressed;
            }

            UpInput = up;
            DownInput = down;
            LeftInput = left;
            RightInput = right;

            HoverLootInput = hoverLootAction != null && hoverLootAction.IsPressed();

            float scroll = 0f;
            if (scrollAction != null)
                scroll = scrollAction.ReadValue<Vector2>().y;

            if (scrollDelay > 0f)
                scrollDelay -= Time.deltaTime;

            if (scrollDelay <= 0f)
            {
                if (scroll > 0f) { OnHotbarUp?.Invoke(); scrollDelay = scrollWheelDelay; }
                else if (scroll < 0f) { OnHotbarDown?.Invoke(); scrollDelay = scrollWheelDelay; }
            }
        }
        else // IF THE BOOL FOR THE NEW INPUT SYSTEM IS FALSE, IT WILL USE THE OLD INPUT SYSTEM INSTEAD
        {
            usingGamepad = false;

            LookHorizontalInput = Input.GetAxisRaw("Mouse X");
            LookVerticalInput = Input.GetAxisRaw("Mouse Y");
            HorizontalInput = Input.GetAxisRaw("Horizontal");
            VerticalInput = Input.GetAxisRaw("Vertical");
            SprintInput = Input.GetKey(KeyCode.LeftShift);
            CrouchInput = Input.GetKey(KeyCode.LeftControl);

            JumpInput = Input.GetKey(KeyCode.Space);
            InventoryInput = Input.GetKey(KeyCode.E);
            ReloadInput = Input.GetKey(KeyCode.R);
            InteractInput = Input.GetKey(KeyCode.F);
            DropInput = Input.GetKey(KeyCode.G);
            PrimarySelectInput = Input.GetMouseButton(0);
            UpInput = Input.GetKey(KeyCode.UpArrow);
            DownInput = Input.GetKey(KeyCode.DownArrow);
            LeftInput = Input.GetKey(KeyCode.LeftArrow);
            RightInput = Input.GetKey(KeyCode.RightArrow);

            if (Input.GetButtonDown("Jump")) OnJump?.Invoke();
            if (Input.GetKeyDown(KeyCode.E)) OnInventory?.Invoke();
            if (Input.GetKeyDown(KeyCode.F)) OnInteract?.Invoke();
            if (Input.GetKeyDown(KeyCode.G)) OnDrop?.Invoke();
            if (Input.GetKeyDown(KeyCode.Escape)) OnPause?.Invoke();

            if (Input.GetMouseButtonDown(0)) OnPrimaryActionDown?.Invoke();
            if (Input.GetMouseButtonDown(1)) OnSecondaryActionDown?.Invoke();
            if (Input.GetMouseButtonDown(2)) OnTerciaryActionDown?.Invoke();

            if (Input.GetMouseButtonUp(0)) OnPrimaryActionUp?.Invoke();
            if (Input.GetMouseButtonUp(1)) OnSecondaryActionUp?.Invoke();
            if (Input.GetMouseButtonUp(2)) OnTerciaryActionUp?.Invoke();

            HoverLootInput = Input.GetKey(KeyCode.LeftAlt);

            float scrollOld = Input.mouseScrollDelta.y;
            if (scrollDelay <= 0)
            {
                if (scrollOld > 0f) { OnHotbarUp?.Invoke(); scrollDelay = scrollWheelDelay; }
                else if (scrollOld < 0f) { OnHotbarDown?.Invoke(); scrollDelay = scrollWheelDelay; }
            }
            else scrollDelay -= Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Tab)) OnHolsterItem?.Invoke();
        }
    }

    void SetupNewInputSystemActions()
    {
        lookAction = new InputAction("Look", InputActionType.Value);
        lookAction.AddBinding("<Mouse>/delta");
        lookAction.AddBinding("<Gamepad>/rightStick");

        moveAction = new InputAction("Move", InputActionType.Value);
        var moveComposite = moveAction.AddCompositeBinding("2DVector");
        moveComposite.With("Up", "<Keyboard>/w");
        moveComposite.With("Down", "<Keyboard>/s");
        moveComposite.With("Left", "<Keyboard>/a");
        moveComposite.With("Right", "<Keyboard>/d");

        var moveComposite2 = moveAction.AddCompositeBinding("2DVector");
        moveComposite2.With("Up", "<Keyboard>/upArrow");
        moveComposite2.With("Down", "<Keyboard>/downArrow");
        moveComposite2.With("Left", "<Keyboard>/leftArrow");
        moveComposite2.With("Right", "<Keyboard>/rightArrow");

        moveAction.AddBinding("<Gamepad>/leftStick");

        sprintAction = new InputAction("Sprint", InputActionType.Button);
        sprintAction.AddBinding("<Keyboard>/leftShift");
        sprintAction.AddBinding("<Gamepad>/leftStickPress");

        crouchAction = new InputAction("Crouch", InputActionType.Button);
        crouchAction.AddBinding("<Keyboard>/leftCtrl");
        crouchAction.AddBinding("<Gamepad>/rightStickPress");

        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");

        inventoryAction = new InputAction("Inventory", InputActionType.Button);
        inventoryAction.AddBinding("<Keyboard>/e");
        inventoryAction.AddBinding("<Gamepad>/buttonNorth");

        reloadAction = new InputAction("Reload", InputActionType.Button);
        reloadAction.AddBinding("<Keyboard>/r");
        reloadAction.AddBinding("<Gamepad>/buttonWest");

        interactAction = new InputAction("Interact", InputActionType.Button);
        interactAction.AddBinding("<Keyboard>/f");
        interactAction.AddBinding("<Gamepad>/buttonWest");

        dropAction = new InputAction("Drop", InputActionType.Button);
        dropAction.AddBinding("<Keyboard>/g");
        dropAction.AddBinding("<Gamepad>/buttonEast");

        primaryAction = new InputAction("Primary", InputActionType.Button);
        primaryAction.AddBinding("<Mouse>/leftButton");
        primaryAction.AddBinding("<Gamepad>/rightTrigger");

        secondaryAction = new InputAction("Secondary", InputActionType.Button);
        secondaryAction.AddBinding("<Mouse>/rightButton");
        secondaryAction.AddBinding("<Gamepad>/leftTrigger");

        terciaryAction = new InputAction("Terciary", InputActionType.Button);
        terciaryAction.AddBinding("<Mouse>/middleButton");
        terciaryAction.AddBinding("<Gamepad>/dpad/up");

        hoverLootAction = new InputAction("HoverLoot", InputActionType.Button);
        hoverLootAction.AddBinding("<Keyboard>/leftAlt");
        hoverLootAction.AddBinding("<Gamepad>/dpad/right");

        scrollAction = new InputAction("Scroll", InputActionType.Value);
        scrollAction.AddBinding("<Mouse>/scroll");

        hotbarUpAction = new InputAction("HotbarUp", InputActionType.Button);
        hotbarUpAction.AddBinding("<Gamepad>/leftShoulder");

        hotbarDownAction = new InputAction("HotbarDown", InputActionType.Button);
        hotbarDownAction.AddBinding("<Gamepad>/rightShoulder");

        holsterAction = new InputAction("Holster", InputActionType.Button);
        holsterAction.AddBinding("<Keyboard>/tab");
        holsterAction.AddBinding("<Gamepad>/dpad/down");

        jumpAction.started += _ => OnJump?.Invoke();
        inventoryAction.started += _ => OnInventory?.Invoke();
        interactAction.started += _ => OnInteract?.Invoke();
        dropAction.started += _ => OnDrop?.Invoke();

        primaryAction.started += _ => OnPrimaryActionDown?.Invoke();
        secondaryAction.started += _ => OnSecondaryActionDown?.Invoke();
        terciaryAction.started += _ => OnTerciaryActionDown?.Invoke();

        primaryAction.canceled += _ => OnPrimaryActionUp?.Invoke();
        secondaryAction.canceled += _ => OnSecondaryActionUp?.Invoke();
        terciaryAction.canceled += _ => OnTerciaryActionUp?.Invoke();

        holsterAction.started += _ => OnHolsterItem?.Invoke();

        hotbarUpAction.started += _ =>
        {
            if (scrollDelay <= 0f)
            {
                OnHotbarUp?.Invoke();
                scrollDelay = scrollWheelDelay;
            }
        };

        hotbarDownAction.started += _ =>
        {
            if (scrollDelay <= 0f)
            {
                OnHotbarDown?.Invoke();
                scrollDelay = scrollWheelDelay;
            }
        };
    }

    void EnableNewInputSystemActions(bool enable)
    {
        if (lookAction == null) return;

        if (enable)
        {
            lookAction.Enable();
            moveAction.Enable();
            sprintAction.Enable();
            crouchAction.Enable();

            jumpAction.Enable();
            inventoryAction.Enable();
            reloadAction.Enable();
            interactAction.Enable();
            dropAction.Enable();

            primaryAction.Enable();
            secondaryAction.Enable();
            terciaryAction.Enable();

            hoverLootAction.Enable();
            scrollAction.Enable();

            hotbarUpAction.Enable();
            hotbarDownAction.Enable();
            holsterAction.Enable();
        }
        else
        {
            lookAction.Disable();
            moveAction.Disable();
            sprintAction.Disable();
            crouchAction.Disable();

            jumpAction.Disable();
            inventoryAction.Disable();
            reloadAction.Disable();
            interactAction.Disable();
            dropAction.Disable();

            primaryAction.Disable();
            secondaryAction.Disable();
            terciaryAction.Disable();

            hoverLootAction.Disable();
            scrollAction.Disable();

            hotbarUpAction.Disable();
            hotbarDownAction.Disable();
            holsterAction.Disable();
        }
    }

    void DisposeNewInputSystemActions()
    {
        lookAction?.Dispose();
        moveAction?.Dispose();
        sprintAction?.Dispose();
        crouchAction?.Dispose();

        jumpAction?.Dispose();
        inventoryAction?.Dispose();
        reloadAction?.Dispose();
        interactAction?.Dispose();
        dropAction?.Dispose();

        primaryAction?.Dispose();
        secondaryAction?.Dispose();
        terciaryAction?.Dispose();

        hoverLootAction?.Dispose();
        scrollAction?.Dispose();

        hotbarUpAction?.Dispose();
        hotbarDownAction?.Dispose();
        holsterAction?.Dispose();
    }
}