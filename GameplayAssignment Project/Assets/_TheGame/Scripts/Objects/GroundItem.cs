using Custom;
using Unity.Netcode;
using UnityEngine;

namespace Custom
{
    [RequireComponent(typeof(Rigidbody))]
    public class GroundItem : MonoBehaviour, IInteractable, IPickableObject
    {// INTERACTABLE OBJECT THAT CAN BE PICKED UP INTO THE INVENTORY OF A CHARACTER

        public Item item;
        public LayerMask excludeLayer;

        [HideInInspector] public Renderer[] renderers;
        [HideInInspector] public Collider[] colliders;

        private void Start()
        {
            renderers = GetComponentsInChildren<Renderer>(true);
            colliders = GetComponentsInChildren<Collider>(true);

            if (renderers.Length == 0 || colliders.Length == 0 || item.obj == null)
            {
                string ObjectName = "Null";
                try { ObjectName = item.obj.ObjectId; } catch { }
                //  MAKE SURE YOU PUT A SCRIPTABLEEEE OBJECT IN THE ITEM.OBJ
                Debug.LogWarning($"Empty Item, deleting {gameObject.name} | {ObjectName} - Santi");
                Destroy(gameObject);
                return;
            }

            foreach (Collider collider in colliders) // SETS UP THE COLLIDERS
            {
                if (collider is MeshCollider)
                {
                    MeshCollider meshCollider = (MeshCollider)collider;
                    meshCollider.convex = true;
                }

                collider.excludeLayers = excludeLayer;
                collider.gameObject.tag = "Item";
            }
        }

        public virtual void Activate(IInteractor interactor) // CALLED TO INTERACT WITH THIS OBJECT
        {
            PlayerController interactorPlayerController = (interactor as PlayerInteraction).GetComponent<PlayerController>();

            interactorPlayerController.playerInventory.InvokeOnPickupItem(this);

            Pickup(interactorPlayerController);
        }

        public void Pickup(PlayerController player) // CALLED TO PICKUP THIS ITEM
        {
            for (int i = 0; i < item.amount; i++)
            {
                bool success = false;
                player.playerInventory.DropInSlots(player.playerInventory.AllSlots, item, true, out success);
            }

            Destroy(this.gameObject);
        }
    }
}