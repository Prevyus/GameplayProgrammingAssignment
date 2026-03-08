using Custom;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Custom
{
    public class PlayerInventory : Inventory
    { // INVENTORY CLASS FOR THE PLAYER, INHERITS FROM THE BASE INVENTORY CLASS
        bool isAlive = true;

        [Header("Refferences")]
        [SerializeField] GameObject InventoryUI;
        [SerializeField] GameObject BackPack;
        [SerializeField] GameObject HotBar;
        [SerializeField] Slot DragSlot;
        [HideInInspector] public Transform handTransform;
        [HideInInspector] public Transform inHandItemTransform;

        [HideInInspector] public List<Slot[]> AllSlots = new List<Slot[]>();

        [HideInInspector] public Slot[] backpackSlots = new Slot[36];
        [HideInInspector] public Slot[] hotbarSlots = new Slot[6];
        [SerializeField] Item[] startingObjectsInBackpack = new Item[36];
        [SerializeField] Item[] startingObjectsInHotbar = new Item[6];

        public int selectedHotbarSlot = -1;
        [HideInInspector] public int previousSelectedHotbarSlot = -1;
        public Object holdingObject = null;

        bool isHoldingSlot = false;

        [HideInInspector] public bool isHoveringLoot;

        public void Death()
        {
            isAlive = false;
        }

        private void Awake()
        {
            AllSlots.Add(hotbarSlots);
            AllSlots.Add(backpackSlots);
        }

        protected void Start()
        { // GETS REFFERENCES FOR THE SLOTS
            for (int i = 0; i < backpackSlots.Length; i++)
            {
                Transform slot = BackPack.transform.GetChild(i);
                Slot backpackSlot = slot.GetComponent<Slot>();
                backpackSlot.Array = backpackSlots;
                backpackSlot.Index = i;
                backpackSlot.item.obj = startingObjectsInBackpack[i].obj;
                backpackSlot.item.amount = startingObjectsInBackpack[i].amount;
                backpackSlots[i] = backpackSlot;
            }
            backpackSlots = UpdateInventory(backpackSlots);
            for (int i = 0; i < hotbarSlots.Length; i++)
            {
                Transform slot = HotBar.transform.GetChild(i);
                Slot hotbarSlot = slot.GetComponent<Slot>();
                hotbarSlot.Array = hotbarSlots;
                hotbarSlot.Index = i;
                hotbarSlot.item.obj = startingObjectsInHotbar[i].obj;
                hotbarSlot.item.amount = startingObjectsInHotbar[i].amount;
                hotbarSlots[i] = hotbarSlot;
            }
            hotbarSlots = UpdateInventory(hotbarSlots);
        }

        private void Update()
        {
            if (handTransform)
            {
                DropTransform.position = inHandItemTransform ? inHandItemTransform.position : handTransform.position;
                DropTransform.rotation = inHandItemTransform ? inHandItemTransform.rotation : handTransform.rotation;
            }

            if (oldInvOpen != inventoryOpen) ToggleInventory(inventoryOpen);

            if (isHoldingSlot) DragHoldingSlot();

            if (isHoveringLoot) HoverLoot();
        }

        public override Slot[] UpdateInventory(Slot[] slots)
        {
            base.UpdateInventory(slots);

            UpdateSelectedHotbarSlot(selectedHotbarSlot, previousSelectedHotbarSlot);

            return slots;
        }

        public override void ToggleInventory(bool open)
        {
            base.ToggleInventory(open);

            InventoryUI.SetActive(open);

            if (inventoryOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            oldInvOpen = open;
        }

        public void UpdateSelectedHotbarSlot(int newSlot, int previousSlot)
        {// CHANGES THE VISUAL FOR SELECTED ITEM IN THE HOTBAR AND GETS THE NEW ITEM REFFERENCE

            if (previousSlot != -1) hotbarSlots[previousSlot].item.image.color = Color.white;
            if (newSlot != -1)
            {
                hotbarSlots[newSlot].item.image.color = Color.blue;
                holdingObject = hotbarSlots[newSlot].item.obj;
            }
            else
            {
                holdingObject = null;
            }
        }

        public void ScrollSelectedHotbar(int direction)
        {// MOVES THE SELECTED ITEM IN THE HOTBAR UP OR DOWN
            previousSelectedHotbarSlot = selectedHotbarSlot;
            int value = selectedHotbarSlot - direction;
            selectedHotbarSlot = value < 0 ? hotbarSlots.Length - 1 : (value >= hotbarSlots.Length ? 0 : value);
            UpdateSelectedHotbarSlot(selectedHotbarSlot, previousSelectedHotbarSlot);
        }

        public void DropSelectedHotbarToGround()
        {// DROPS THE ITEM THAT THE PLAYER IS CURRENTLY HOLDING
            bool success = false;
            hotbarSlots = RemoveFromSlot(hotbarSlots, selectedHotbarSlot, 1, true, out success);
        }

        public void SetSelectedHotbar(int index)
        {// SETS THE SELECTED ITEM WITH A SPECIFIC NUMBER, THIS IS FOR PRESSING 1 THROUGH 6 TO GET A SPECIFIC SLOT
            previousSelectedHotbarSlot = selectedHotbarSlot;
            index--;
            selectedHotbarSlot = (index == -2 ? -1 : (index) % hotbarSlots.Length);
            //Debug.Log(index == -2 ? "Holstered" : $"Set {selectedHotbarSlot}");
            UpdateSelectedHotbarSlot(selectedHotbarSlot, previousSelectedHotbarSlot);
        }

        public void OnClickedSlot(int clickType)
        {// THIS ACTIVATES THE DRAGGING MECHANIC INSIDE THE INVENTORY
            if (!inventoryOpen || DragSlot.isActiveAndEnabled) return;

            var pos = Input.mousePosition;
            var slots = GetSlotsUnderPointer(pos);

            foreach (var slot in slots)
            {
                if (slot.isDraggerSlot || slot.item.obj == null) continue;
                isHoldingSlot = true;

                DragSlot.gameObject.SetActive(true);
                DragSlot.draggingType = clickType;
                DragSlot.draggingSlot = slot;
                DragSlot.item.obj = slot.item.obj;
                DragSlot.item.image.texture = slot.item.image.texture;

                if (clickType == 0) DragSlot.item.amount = slot.item.amount;
                else if (clickType == 1) DragSlot.item.amount = 1;
                else if (clickType == 2) DragSlot.item.amount = (slot.item.amount + 1) / 2;

                break;
            }
        }

        void DragHoldingSlot()
        {
            DragSlot.MoveTo(Input.mousePosition);
        }

        public void OnUnclickedSlot(int clickType)
        {// CALLED WHEN LET GO OF AN ITEM AFTER DRAGGING IT, GETS THE SLOT THAT IT LANDED INTO AND TRIED TO ADD THE DRAGGED ITEM
            if (isHoldingSlot)
            {
                if (DragSlot.draggingType != clickType) return;
                bool foundValidSlot = false;
                bool switchedSlots = false;
                bool dropped = false;
                var pos = Input.mousePosition;
                List<Slot> slots = GetSlotsUnderPointer(pos);

                foreach (Slot slot in slots)
                {
                    if (slot.isDraggerSlot) continue;

                    foundValidSlot = true;
                    (Slot[] From, Slot[] To) SwitchingSlots = (DragSlot.draggingSlot.Array, slot.Array);
                    SwitchingSlots = SwitchSlots(SwitchingSlots.From, DragSlot.draggingSlot.Index, slot.Array, slot.Index, clickType, out switchedSlots);

                    break;
                }
                if (!foundValidSlot)
                {
                    DragSlot.draggingSlot.Array = RemoveFromSlot(DragSlot.draggingSlot.Array, DragSlot.draggingSlot.Index, clickType, true, out dropped);
                }

                DragSlot.gameObject.SetActive(false);
                DragSlot.MoveTo(new Vector2(-10000, -10000));
                DragSlot.draggingSlot = null;
                DragSlot.item.obj = null;
                DragSlot.item.amount = 0;
                DragSlot.item.image.texture = emptyItemImage;
                isHoldingSlot = false;
            }
        }

        public void HoverLoot()
        { // MECHANIC THAT LETS YOU QUICKLY TRANSFER ITEMS INTO ANOTHER GROUP OF SLOTS BY HOVERING OVER THE ITEMS
            var pos = Input.mousePosition;
            List<Slot> slots = GetSlotsUnderPointer(pos);
            foreach (Slot slot in slots)
            {
                if (slot.isDraggerSlot) continue;
                if (slot.item.obj == null) continue;

                List<Slot[]> slotDropInOrder = new List<Slot[]>();
                slotDropInOrder.Clear();
                slotDropInOrder.Add(slot.Array == hotbarSlots ? backpackSlots : hotbarSlots);

                Slot oldSlot = CopySlot(slot);

                int amount = slot.item.amount;
                bool success = false;
                for (int i = 0; i < amount; i++)
                {
                    slotDropInOrder[0] = DropInSlots(slotDropInOrder, slot.item, false, out success);
                    if (success)
                    {
                        bool removeSuccess = false;
                        oldSlot.Array = RemoveFromSlot(oldSlot.Array, oldSlot.Index, 1, false, out removeSuccess);
                    }
                }
            }
        }

        List<Slot> GetSlotsUnderPointer(Vector2 screenPos)
        { // GETS THE GROUP OF SLOTS THAT THE SLOT THAT IS IN THE SCREEN POSITION BELONGS TO
            var results = new List<RaycastResult>();
            var data = new PointerEventData(EventSystem.current);
            data.position = screenPos;
            EventSystem.current?.RaycastAll(data, results);

            var list = new List<Slot>();
            var seen = new HashSet<Slot>();
            for (int i = 0; i < results.Count; i++)
            {
                var go = results[i].gameObject;
                if (!go) continue;
                var slot = go.GetComponentInParent<Slot>();
                if (slot != null && seen.Add(slot)) list.Add(slot);
            }
            return list;
        }
    }
}