using JetBrains.Annotations;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Vowgan.Inventory.UI
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ItemButtonUi : BaseInventoryBehaviour
    {
        public InventoryItem Item => _item;
        
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TextMeshProUGUI _itemName;
        [SerializeField] private InventoryItem _item;


        /// <summary>
        /// Initialize the menu button after being created.
        /// </summary>
        public void _Init(PickupInventoryMenu menu, InventoryItem item)
        {
            Menu = menu;
            _item = item;

            _itemName.text = item.ItemName;
            _itemIcon.sprite = item.Icon;
        }

        /// <summary>
        /// Called when the UI Button is clicked. Assigned manually in the editor.
        /// </summary>
        [PublicAPI]
        public void _OnClick()
        {
            Menu._SelectItem(_item);
        }
    }
}