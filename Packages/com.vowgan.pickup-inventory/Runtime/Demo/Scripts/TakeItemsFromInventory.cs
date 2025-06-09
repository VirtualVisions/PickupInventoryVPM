
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;


namespace Vowgan.Inventory.Demo
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TakeItemsFromInventory : BaseInventoryBehaviour
    {
        
        [Tooltip("Names of items to look for and remove.")]
        [SerializeField] private string[] _itemsToTake = new[] { "Apple", "Apple", "Apple" };
        [Tooltip("Which storage to take from.")]
        [SerializeField] private PickupInventoryStorage _inventoryStorage;
        
        
        public override void Interact()
        {
            DataList foundItemIndexList = new DataList();

            string[] names = _inventoryStorage._GetItemNames();

            foreach (string itemName in _itemsToTake)
            {
                int index = Array.IndexOf(names, itemName);
                if (index == -1) continue;
                foundItemIndexList.Add(index);
            }

            if (foundItemIndexList.Count != _itemsToTake.Length)
            {
                Debug.Log("Did not find all required items in inventory.");
                return;
            }

            DataList foundItems = new DataList();

            for (int i = 0; i < foundItemIndexList.Count; i++)
            {
                foundItems.Add(_inventoryStorage.ItemList[foundItemIndexList[i].Int]);
            }
            
            for (int i = 0; i < foundItems.Count; i++)
            {
                InventoryItem item = (InventoryItem)foundItems[i].Reference;
                _inventoryStorage._RemoveItem(item);
            }
        }
    }
}