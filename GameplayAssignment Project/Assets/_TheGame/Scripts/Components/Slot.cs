using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Custom;

namespace Custom
{
    [System.Serializable]
    public struct Item
    { // STRUCT THAT STORES A SCRIPTABLE OBJECT, ITS AMOUNT AND THE IMAGE COMPONENT IT SHOULD HAVE
        public Object obj;
        public int amount;
        public RawImage image;
    }

    public class Slot : MonoBehaviour
    { // SLOT USED IN THE INVENTORY SYSTEM, STORES ITEMS
        public Item item = new Item();
        [HideInInspector] public Slot[] Array;
        [HideInInspector] public int Index;
        public bool isDraggerSlot = false;
        [HideInInspector] public Slot draggingSlot = null;
        [HideInInspector] public int draggingType = 0; // 0 = stack, 1 = single, 2 = half
        public TextMeshProUGUI amountText;

        int oldAmount = 0;

        private void Update()
        {
            if (oldAmount != item.amount) OnAmountChanged();
        }

        void OnAmountChanged()
        { // IF THE AMOUNT IN THE SLOT IS 1 IT DEACTIVATES THE AMOUNT TEXT, OTHERWISE IT ENABLES IT AND CHANGES THE NUMBER
            if (item.amount < 2) amountText.gameObject.SetActive(false);
            else if (oldAmount < 2) amountText.gameObject.SetActive(true);
            amountText.text = $"{item.amount}";
            oldAmount = item.amount;
        }

        public void MoveTo(Vector2 screenPos)
        {// MOVES THIS SLOT INTO A POSITION ON THE SCREEN, USED ON THE DRAG SLOT (A VISUAL FOR WHEN YOU MOVE ITEMS AROUND IN INVENTORIES)
            var rt = (RectTransform)transform;
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null || rt.parent == null) return;
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)rt.parent,
                screenPos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out localPoint
            );
            rt.anchoredPosition = localPoint;
        }
    }
}