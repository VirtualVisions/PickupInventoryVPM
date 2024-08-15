﻿using UdonSharp;

namespace Vowgan.Inventory
{
    public class PickupProxy : UdonSharpBehaviour
    {

        public InventoryItem Item;
        public PickupInventory Inventory;
        
        public bool InsertingToInventory;
        

        public override void OnPickup()
        {
            if (!Item.JustSpawned) return;
            Item.JustSpawned = false;
            
            Item._RunFirstPickupAfterSpawn();
        }

        public override void OnDrop()
        {
            if (!InsertingToInventory) return;
            InsertingToInventory = false;
            Inventory._AddItem(Item);
        }
    }
}