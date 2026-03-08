using Custom;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Custom
{
    public class Inventory : MonoBehaviour
    { // BASE CLASS FOR THE INVENTORY SYSTEM, COULD BE USED FOR CHARACTER INVENTORIES BUT ALSO CONTAINERS OR OTHER STUFF

        public Transform DropTransform;
        public Transform dropParent;
        public bool inventoryOpen = false;
        [SerializeField] float dropImpulse = 1;
        public Texture emptyItemImage;
        [HideInInspector] public bool oldInvOpen = false;

        public event Action<GroundItem> OnPickupItem;

        public virtual Slot[] UpdateInventory(Slot[] slots)
        {// UPDATES THE INVENTORIES VALUES
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item.obj == null) continue;
                slots[i].item.image.texture = slots[i].item.obj.itemImage;
            }
            return slots;
        }

        public virtual void ToggleInventory(bool open)
        {
            inventoryOpen = open;
        }

        public virtual Slot[] DropInSlots(List<Slot[]> allSlots, Item item, bool dropOnFail, out bool success)
        {// THIS FINDS AN AVAILABLE SPOT AND ADDS THE ITEM INTO THE INVENTORY
            success = false;
            int successfulSlots = -1;
            (Slot[] Array, int Index, bool found) matchingSlot = (new Slot[0], 0, false);

            for (int s = 0; s < allSlots.Count; s++)
            {
                for (int i = 0; i < allSlots[s].Length; i++) // LOOKS FOR SPOT WITH EXISTING SAME ITEM
                {
                    if (allSlots[s][i].item.obj == null) continue;
                    if (allSlots[s][i].item.obj == item.obj && allSlots[s][i].item.amount < allSlots[s][i].item.obj.stackSize)
                    {
                        matchingSlot = (allSlots[s], i, true);
                        break;
                    }
                }
                if (matchingSlot.found) break;
            }

            if (matchingSlot.found) // ADDS THE ITEM TO THAT STACK
            {
                bool AddToSlotSuccess = false;
                matchingSlot.Array = AddToSlot(matchingSlot.Array, matchingSlot.Index, item.obj, out AddToSlotSuccess);
                success = true;
                return matchingSlot.Array;
            }

            for (int s = 0; s < allSlots.Count; s++) // LOOKS FOR THE NEXT EMPTY SPOT AND ADDS THE ITEM
            {
                for (int i = 0; i < allSlots[s].Length; i++)
                {
                    bool AddToSlotSuccess = false;
                    allSlots[s] = AddToSlot(allSlots[s], i, item.obj, out AddToSlotSuccess);
                    if (!AddToSlotSuccess) continue;

                    success = true;
                    return allSlots[s];
                }
            }

            if (!success && dropOnFail) // IF IT FAILED TO FIND ANY SPOT AVAILABLE IT DROPS THE ITEM INTO THE GROUND
            {
                DropObject(item);
            }
            return new Slot[0];
        }

        public virtual Slot[] AddToSlot(Slot[] slots, int index, Object obj, out bool success)
        {// ADDS ITEM INTO A SPECIFIC SLOT IN AN INVENTORY
            success = false;
            if (index < 0 || index >= slots.Length) return slots;
            if (slots[index].item.obj != obj && slots[index].item.obj != null) return slots;
            if (slots[index].item.obj == obj && slots[index].item.amount >= obj.stackSize) return slots;
            slots[index].item.obj = obj;
            slots[index].item.amount++;
            slots[index].item.image.texture = obj.itemImage;
            slots[index].Array = slots;
            slots[index].Index = index;

            slots = UpdateInventory(slots);
            success = true;
            return slots;
        }

        public virtual (Slot[], Slot[]) SwitchSlots(Slot[] SlotsFrom, int indexFrom, Slot[] SlotsTo, int indexTo, int switchType, out bool success)
        {// MOVES OR SWITCHES THE CONTENTS FROM ONE SLOT INTO ANOTHER
            success = false;
            (Slot[] resultSlotsFrom, Slot[] resultSlotsTo) result = (SlotsFrom, SlotsTo);
            if (indexTo < 0 || indexTo >= SlotsTo.Length) return result;
            if (indexFrom < 0 || indexFrom >= SlotsFrom.Length) return result;

            Slot from = SlotsFrom[indexFrom];
            Slot to = SlotsTo[indexTo];

            if (from.item.obj == null) return result;

            if (from.item.obj == to.item.obj || (switchType != 0 && to.item.obj == null))
            {// IF THE BOTH SLOTS HAVE THE SAME ITEM OR IF THE RECEIVING SLOT IS EMPTY, IT SIMPLY ADDS THE AMOUNT TO THE RECEIVING SLOT
                int amount = 0;
                if (switchType == 0) amount = from.item.amount;
                if (switchType == 1) amount = 1;
                if (switchType == 2) amount = (from.item.amount + 1) / 2;

                for (int i = 0; i < amount; i++)
                {
                    bool AddSuccess = false;
                    SlotsTo = AddToSlot(SlotsTo, indexTo, from.item.obj, out AddSuccess);
                    if (!AddSuccess) break;

                    bool RemoveSuccess = false;
                    SlotsFrom = RemoveFromSlot(SlotsFrom, indexFrom, 1, false, out RemoveSuccess);

                    success = true;
                }
            }
            else if (switchType == 0)
            {// IF THE SENDING SLOT AND RECEIVING SLOT HAVE DIFFERENT ITEMS, IT SWITCHES THE STACKS
                Object toObj = to.item.obj;
                int toAmount = to.item.amount;
                Texture toTexture = to.item.image.texture;

                to.item.obj = from.item.obj;
                to.item.amount = from.item.amount;
                to.item.image.texture = from.item.image.texture;

                from.item.obj = toObj;
                from.item.amount = toAmount;
                from.item.image.texture = toTexture;
                success = true;
            }

            // UPDATES THE INVENTORY WITH THE RESULTING CONFIGURATION
            SlotsFrom = UpdateInventory(SlotsFrom);
            SlotsTo = UpdateInventory(SlotsTo);
            result = (SlotsFrom, SlotsTo);
            return result;
        }

        public virtual Slot[] RemoveFromSlot(Slot[] slots, int index, int removeType, bool dropToGround, out bool success)
        { // REMOVES ITEM FROM A SPECIFIC SLOT, OPTIONALLY IT CAN DROP IT TO THE GROUND
            success = false;
            Object obj;
            if (index < 0 || index >= slots.Length) return slots;
            if (slots[index].item.obj == null) return slots;

            obj = slots[index].item.obj;

            Item itemToDrop = slots[index].item;

            if (removeType == 1) itemToDrop.amount = 1;
            else if (removeType == 2) itemToDrop.amount = (itemToDrop.amount + 1) / 2;

            if (dropToGround) DropObject(itemToDrop);

            if (slots[index].item.amount > 0)
            {// DEPENDING ON THE REMOVING TYPE, IT REMOVES THE WHOLE STACK, A SINGLE ITEM OR HALF THE STACK
                if (removeType == 0) slots[index].item.amount = 0;
                if (removeType == 1) slots[index].item.amount = slots[index].item.amount - 1;
                if (removeType == 2) slots[index].item.amount = slots[index].item.amount - (slots[index].item.amount + 1) / 2;
            }
            else slots[index].item.amount = 0;

            if (slots[index].item.amount == 0)
            {
                slots[index].item.obj = null;
                slots[index].item.image.texture = emptyItemImage;
            }

            slots = UpdateInventory(slots);
            success = true;
            return slots;
        }

        public virtual Slot CopySlot(Slot slot)
        {// GETS THE VALUES FROM ONE SLOT AND RETURNS IT
            Slot result = new Slot();

            result.item.obj = slot.item.obj;
            result.item.amount = slot.item.amount;
            result.Array = slot.Array;
            result.Index = slot.Index;
            result.isDraggerSlot = slot.isDraggerSlot;
            result.draggingSlot = slot.draggingSlot;
            result.draggingType = slot.draggingType;

            return result;
        }

        public virtual void DropObject(Item item)
        {// DROPS AN ITEM INTO THE GROUND/WORLD
            GameObject prefabIndex = item.obj.InGroundPrefab;
            if (!prefabIndex) return;

            Vector3 pos = DropTransform.position;
            Quaternion rot = DropTransform.rotation;
            Vector3 vel = transform.forward * dropImpulse;

            GameObject go = Instantiate(prefabIndex, pos, rot, dropParent);
            GroundItem groundItem = go.GetComponent<GroundItem>();
            if (groundItem != null) groundItem.item.amount = item.amount;

            Rigidbody rb = go.GetComponent<Rigidbody>();
            if (rb != null) rb.AddForce(vel, ForceMode.VelocityChange);
        }

        public virtual void InvokeOnPickupItem(GroundItem item)
        {// CALLED BY THE GROUND ITEM WHEN THE PLAYER PICKS IT UP
            OnPickupItem?.Invoke(item);
        }
    }

}