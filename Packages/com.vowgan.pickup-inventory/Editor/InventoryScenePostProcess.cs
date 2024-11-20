using UdonSharpEditor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using VRC.Udon;

namespace Vowgan.Inventory
{
    public class InventoryScenePostProcess : MonoBehaviour
    {
        
        [PostProcessScene(-100)]
        public static void PostProcessScene()
        {
            PickupInventory inventory = FindObjectOfType<PickupInventory>();

            InventoryItem[] items = FindObjectsByType<InventoryItem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            foreach (InventoryItem item in items)
            {
                if (!item.Pickup) continue;

                PickupProxy proxy = item.Pickup.gameObject.AddUdonSharpComponent<PickupProxy>();
                proxy.Item = item;
                proxy.Inventory = inventory;

                if (EditorApplication.isPlaying)
                {
                    UdonBehaviour udonProxy = UdonSharpEditorUtility.GetBackingUdonBehaviour(proxy);
                    UdonManager.Instance.RegisterUdonBehaviour(udonProxy);
                }
                
            }
            
        }
    }
}