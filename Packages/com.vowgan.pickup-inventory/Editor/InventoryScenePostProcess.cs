using System.Collections.Generic;
using UdonSharpEditor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Vowgan.Inventory.UI;
using VRC.Udon;

namespace Vowgan.Inventory
{
    public class InventoryScenePostProcess : MonoBehaviour
    {
        
        [PostProcessScene(-100)]
        public static void PostProcessScene()
        {
            PickupInventoryMenu menu = FindObjectOfType<PickupInventoryMenu>();
            PickupItemLibrary library = FindObjectOfType<PickupItemLibrary>();

            if (!menu || !library)
                return;

            BaseInventoryBehaviour[] behaviours =
                FindObjectsByType<BaseInventoryBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            foreach (BaseInventoryBehaviour behaviour in behaviours)
            {
                behaviour.Menu = menu;
                behaviour.Library = library;
            }
            
            
            InventoryItem[] items =
                FindObjectsByType<InventoryItem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            library.Items = items;
            
            
            foreach (InventoryItem item in items)
            {
                if (!item.Pickup) continue;
                
                PickupProxy proxy = item.Pickup.gameObject.AddUdonSharpComponent<PickupProxy>();
                proxy.Item = item;
                
                if (EditorApplication.isPlaying)
                {
                    UdonBehaviour udonProxy = UdonSharpEditorUtility.GetBackingUdonBehaviour(proxy);
                    UdonManager.Instance.RegisterUdonBehaviour(udonProxy);
                }
            }

            HashSet<int> existingIds = new HashSet<int>();
            PickupInventoryStorage[] inventories = FindObjectsByType<PickupInventoryStorage>(FindObjectsSortMode.None);
            foreach (PickupInventoryStorage inventory in inventories)
            {
                int id = inventory.StorageId;
                if (id >= 0) id = -1;
                
                while (existingIds.Contains(id))
                    id--;

                existingIds.Add(id);
                inventory.StorageId = id;
            }
        }
    }
}