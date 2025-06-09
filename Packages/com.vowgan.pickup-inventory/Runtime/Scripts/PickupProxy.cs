using UnityEngine;

namespace Vowgan.Inventory
{
    /// <summary>
    /// A generalized proxy for the VRCPickup object.
    /// </summary>
    public class PickupProxy : BaseInventoryBehaviour
    {

        public bool IsHeld => _isHeld;
        
        [Tooltip("The item this is proxying for.")]
        public InventoryItem Item;
        [Tooltip("Which inventory storage this item is waiting to drop into.")]
        public PickupInventoryStorage TargetInventory;
        
        private bool _isHeld;

        public override void OnPickup()
        {
            _isHeld = true;
            
            if (!Item._justSpawned) return;
            Item._justSpawned = false;
            Item._RunFirstPickupAfterSpawn();
        }

        public override void OnDrop()
        {
            _isHeld = false;
            
            if (!TargetInventory) return;
            TargetInventory._AddItem(Item);
            TargetInventory = null;
        }
    }
}