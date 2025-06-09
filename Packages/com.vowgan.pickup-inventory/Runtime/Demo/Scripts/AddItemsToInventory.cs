using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Vowgan.Inventory.Demo
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class AddItemsToInventory : UdonSharpBehaviour
    {
        [Tooltip("Items to be added to the inventory.")]
        [SerializeField] private InventoryItem[] _items;
        [Tooltip("Which storage to store items inside.")]
        [SerializeField] private PickupInventoryStorage _inventoryStorage;
        [UdonSynced, SerializeField, ReadoutOnly] private bool _used;
        
        private void Start()
        {
            // Hide the target items by default if you're the owner.
            foreach (InventoryItem item in _items)
            {
                if (Networking.LocalPlayer.IsOwner(item.gameObject))
                    item._Hide();
            }
        }

        public override void OnDeserialization()
        {
            // Disable the interaction if the button has already been pressed.
            DisableInteractive = _used;
        }

        public override void Interact()
        {
            foreach (InventoryItem item in _items)
                _inventoryStorage._AddItem(item);

            _used = true;
            RequestSerialization();
            OnDeserialization();
        }
    }
}