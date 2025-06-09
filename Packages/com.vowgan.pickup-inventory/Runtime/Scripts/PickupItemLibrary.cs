
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

namespace Vowgan.Inventory
{
    /// <summary>
    /// Library storing all items and their storage IDs.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PickupItemLibrary : BaseInventoryBehaviour
    {
        public PickupInventoryStorage LocalStorage;
        public InventoryItem[] Items;
        
        /// <summary>
        /// Get all items that are stored in a current inventory by given ID.
        /// </summary>
        public DataList _GetItemsWithID(int id)
        {
            DataList items = new DataList();
            
            for (int i = 0; i < Items.Length; i++)
            {
                InventoryItem item = Items[i];
                if (item._GetStorageId() == id)
                    items.Add(item);
            }
            
            return items;
        }

        /// <summary>
        /// Returns true if the provided list contains the correct collection of items that have a given ID.
        /// Used to determine if the menu should rebuild, ignoring item order.
        /// </summary>
        public bool _ContainsAllItemsWithID(DataList list, int id)
        {
            DataList realList = _GetItemsWithID(id);
            for (int i = 0; i < list.Count; i++)
            {
                DataToken value = list[i];
                if (!realList.Contains(value))
                    return false;
                
                realList.Remove(value);
            }

            // If any values exist that weren't in the original list, the list does not contain all correct values.
            return realList.Count == 0;
        }
        
    }
}