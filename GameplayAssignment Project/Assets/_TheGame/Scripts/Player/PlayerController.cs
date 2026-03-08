using UnityEngine;
using UnityEngine.InputSystem;

namespace Custom
{
    public class PlayerController : MonoBehaviour
    { // CLASS THAT CONNECTS VARIABLES BETWEEN ALL COMPONENTS THAT THE PLAYER USES

        [HideInInspector] public bool isAlive = true;

        [HideInInspector] public PlayerInput playerInput;
        [HideInInspector] public PlayerMovement playerMovement;
        [HideInInspector] public PlayerInventory playerInventory;
        [HideInInspector] public PlayerItemHolder playerItemHolder;
        [HideInInspector] public PlayerWeaponHandler playerWeaponHandler;
        [HideInInspector] public PlayerInteraction playerInteraction;
        [HideInInspector] public CharacterAnimation playerAnimation;
        [HideInInspector] public PlayerHealth playerHealth;
        [HideInInspector] public PlayerUI playerUI;
        [HideInInspector] public CharacterModel playerModel;
        [HideInInspector] public CharacterController characterController;

        [HideInInspector] public Camera playerCamera;
        public Transform EyePosition;

        void Death() // CALLED WHEN THE PLAYER DIES
        {
            playerInventory.DropSelectedHotbarToGround();
            playerItemHolder.holdableItem.gameObject.SetActive(false);

            isAlive = false;

            playerInput.Death();
            playerMovement.Death();
            playerInventory.Death();
            playerItemHolder.Death();
            playerWeaponHandler.Death();
            playerInteraction.Death();
            playerAnimation.Death();

            GameRoot.Instance.PlayerDeath();
        }

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            playerMovement = GetComponent<PlayerMovement>();
            playerInventory = GetComponent<PlayerInventory>();
            playerItemHolder = GetComponent<PlayerItemHolder>();
            playerWeaponHandler = GetComponent<PlayerWeaponHandler>();
            playerAnimation = GetComponentInChildren<CharacterAnimation>();
            playerHealth = GetComponent<PlayerHealth>();
            playerUI = GetComponent<PlayerUI>();
            playerCamera = GetComponentInChildren<Camera>();
            playerInteraction = GetComponent<PlayerInteraction>();
            playerAnimation.animator = GetComponentInChildren<Animator>();
            playerModel = GetComponent<CharacterModel>();
            characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            SetComponentRefferences();
            SubscribeEvents();
        }

        void Update()
        {
            ApplyMovementInput();
            Movement();
            Animation();
            Inventory();
            UI();
            ItemHolder();
            WeaponHandler();
        }

        void SetComponentRefferences()
        {
            playerMovement.EyePosition = EyePosition;
            playerInteraction.playerController = this;

            playerInventory.handTransform = playerItemHolder.rightHand;
        }

        void SubscribeEvents()
        {
            playerHealth.OnDeath += Death;
            playerInput.OnJump += playerMovement.Jump;
            playerInput.OnInventory += () => playerInventory.ToggleInventory(!playerInventory.inventoryOpen);
            playerInput.OnInventory += () => playerUI.OnInventory(playerInventory.inventoryOpen);
            playerInput.OnInteract += playerInteraction.HandleInteractEvent;
            playerInventory.OnPickupItem += playerAnimation.OnPickup;
            playerMovement.OnJump += playerAnimation.OnJump;
            playerHealth.OnDealtDamage += playerAnimation.OnDealtDamage;
            playerInput.OnHotbarUp += () => playerInventory.ScrollSelectedHotbar(1);
            playerInput.OnHotbarDown += () => playerInventory.ScrollSelectedHotbar(-1);
            playerInput.OnHotbarButton += playerInventory.SetSelectedHotbar;
            playerInput.OnHolsterItem += () => playerInventory.SetSelectedHotbar(-1);
            playerInput.OnDrop += playerInventory.DropSelectedHotbarToGround;
            playerInput.OnPrimaryActionDown += () => playerInventory.OnClickedSlot(0);
            playerInput.OnPrimaryActionUp += () => playerInventory.OnUnclickedSlot(0);
            playerInput.OnSecondaryActionDown += () => playerInventory.OnClickedSlot(1);
            playerInput.OnSecondaryActionUp += () => playerInventory.OnUnclickedSlot(1);
            playerInput.OnTerciaryActionDown += () => playerInventory.OnClickedSlot(2);
            playerInput.OnTerciaryActionUp += () => playerInventory.OnUnclickedSlot(2);
            playerInput.OnSecondaryActionDown += playerItemHolder.OnAimDown;
            playerInput.OnSecondaryActionUp += playerItemHolder.OnAimUp;
            playerInput.OnPrimaryActionDown += playerWeaponHandler.OnAttackDown;
            playerInput.OnPrimaryActionUp += playerWeaponHandler.OnShootUp;
            playerItemHolder.OnNewHoldableItem += playerWeaponHandler.OnNewHoldableItem;
        }

        void ApplyMovementInput()
        {
            playerMovement.MouseHorizontalInput = playerInput.LookHorizontalInput;
            playerMovement.MouseVerticalInput = playerInput.LookVerticalInput;
            playerMovement.HorizontalInput = playerInput.HorizontalInput;
            playerMovement.VerticalInput = playerInput.VerticalInput;
            playerMovement.SprintInput = playerInput.SprintInput;
            playerMovement.CrouchInput = playerInput.CrouchInput;
        }

        void Movement()
        {
            if (!playerInventory.inventoryOpen) playerMovement.LookMovement();
            playerMovement.WalkMovement();
        }

        void Animation()
        {
            playerAnimation.rightHandOccupied = playerItemHolder.holdableItem != null && playerItemHolder.holdableItem.armSets.Count > 0 ? playerItemHolder.holdableItem.armSets[playerItemHolder.armSet].rightElbow != null && playerItemHolder.holdableItem.armSets[playerItemHolder.armSet].rightHand != null : false;
            playerAnimation.leftHandOccupied = playerItemHolder.holdableItem != null && playerItemHolder.holdableItem.armSets.Count > 0 ? playerItemHolder.holdableItem.armSets[playerItemHolder.armSet].leftElbow != null && playerItemHolder.holdableItem.armSets[playerItemHolder.armSet].leftHand != null : false;

            playerAnimation.isAlive = playerHealth.isAlive;
            playerAnimation.isIdle = playerMovement.HorizontalInput == 0 && playerMovement.VerticalInput == 0;
            playerAnimation.isWalking = !playerInput.SprintInput && !playerAnimation.isIdle;
            playerAnimation.isRunning = playerMovement.isRunning && !playerAnimation.isIdle;
            playerAnimation.isCrouched = playerInput.CrouchInput;
            playerAnimation.TargetForwardMovement = playerMovement.VerticalInput;
            playerAnimation.TargetSidewaysMovement = playerMovement.HorizontalInput;
            playerAnimation.isGrounded = playerMovement.isGrounded;
            playerAnimation.isLanding = playerMovement.isLanding; }

        void Inventory()
        {
            playerItemHolder.holdingObject = playerInventory.holdingObject;
            playerInventory.isHoveringLoot = playerInput.HoverLootInput;
            playerInventory.inHandItemTransform = playerItemHolder.holdableItem ? playerItemHolder.holdableItem.Item : null;
        }

        void UI()
        {
            playerUI.stamina = playerMovement.stamina;
            playerUI.health = playerHealth.Health;
        }

        void ItemHolder()
        {
            playerItemHolder.LookHorizontalInput = playerInput.LookHorizontalInput;
            playerItemHolder.LookVerticalInput = playerInput.LookVerticalInput;
        }

        void WeaponHandler()
        {
            playerWeaponHandler.inventoryOpen = playerInventory.inventoryOpen;
        }
    }
}


