
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

namespace Vowgan.Inventory
{
    /// <summary>
    /// Default sizes available for inventory items.
    /// </summary>
    public enum InventoryItemSize
    {
        Small = 0,
        Medium = 1,
        Large = 2,
        ExtraLarge = 3,
    }
    
    /// <summary>
    /// Generalized Utility functions made for the Inventory system.
    /// </summary>
    public class InventoryUtility : UdonSharpBehaviour
    {
        public static bool IsEquivalent(DataList list1, DataList list2)
        {
            if (!Utilities.IsValid(list1) || !Utilities.IsValid(list2))
                return false;

            if (list1.Count != list2.Count)
                return false;

            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i] != list2[i])
                    return false;
            }

            return true;
        }

        public static string _ItemSizeName(InventoryItemSize size)
        {
            string[] INVENTORY_ITEM_SIZE = new string[]
            {
                "Small",
                "Medium",
                "Large",
                "Extra Large",
            };

            return INVENTORY_ITEM_SIZE[(int)size];
        }

    }
}