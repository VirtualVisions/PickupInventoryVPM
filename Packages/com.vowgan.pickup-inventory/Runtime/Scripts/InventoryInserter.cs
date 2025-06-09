using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Vowgan.Inventory
{
    [RequireComponent(typeof(Rigidbody))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class InventoryInserter : BaseInventoryBehaviour
    {
        
        /// <summary>
        /// Whether to highlight the inserter to show an item is ready to insert.
        /// </summary>
        public virtual void _Highlight(bool value)
        {
            if (value)
                Menu.Sound._PlayInsertActive();
            else
                Menu.Sound._PlayInsertInactive();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!Utilities.IsValid(other)) return;
            if (!Networking.LocalPlayer.IsOwner(other.gameObject)) return;

            PickupProxy proxy = other.GetComponent<PickupProxy>();
            if (!proxy) return;
            if (!proxy.IsHeld) return;
            
            if (Menu.CurrentInventory.MaxItemCount != 0 && 
                Menu.CurrentInventory.ItemCount >= Menu.CurrentInventory.MaxItemCount)
            {
                int itemCount = Menu.CurrentInventory.ItemCount;
                int maxCount = Menu.CurrentInventory.MaxItemCount;
                
                Menu.Modal._ShowModal(
                    $"Inventory Full - {itemCount}/{maxCount}", 
                    "The inventory is at full capacity. Please remove some items before attempting to add more.");
                return;
            }
            
            if ((int)proxy.Item.ItemSize > (int)Menu.CurrentInventory.MaxItemSize)
            {
                string itemName = proxy.Item.ItemName;
                string itemSize = proxy.Item._GetSizeName();
                string maxSize = InventoryUtility._ItemSizeName(Menu.CurrentInventory.MaxItemSize);
                
                Menu.Modal._ShowModal(
                    $"{itemName} is too big.",
                    $"The item {itemName} ({itemSize}) is too big.\nMust be {maxSize} or smaller.");
                return;
            }
            
            proxy.TargetInventory = Menu.CurrentInventory;

            _Highlight(true);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!Utilities.IsValid(other)) return;
            if (!Networking.LocalPlayer.IsOwner(other.gameObject)) return;

            PickupProxy proxy = other.GetComponent<PickupProxy>();
            if (!proxy) return;
            proxy.TargetInventory = null;

            _Highlight(false);
        }
    }
}