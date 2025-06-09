using System;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;

namespace Vowgan.Inventory
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PickupInventoryStorage : BaseInventoryBehaviour
    {
        public DataList ItemList => _itemList;
        public int ItemCount => ItemList.Count;
        public InventoryItemSize MaxItemSize => _maxItemSize;
        public int MaxItemCount => _maxItemCount;
        
        /// <summary>
        /// Compile an array of all item names within this storage.
        /// </summary>
        public string[] _GetItemNames()
        {
            string[] names = new string[ItemCount];
            for (int i = 0; i < ItemCount; i++)
            {
                names[i] = ((InventoryItem)_itemList[i].Reference).ItemName;
            }

            return names;
        }
        
        [Tooltip("The currently assigned storage ID.")]
        [ReadoutOnly] public int StorageId;
        [Tooltip("The max size of item that can be stored in this inventory.")]
        [SerializeField] private InventoryItemSize _maxItemSize = InventoryItemSize.Medium;
        [Tooltip("Maximum amount of items that can be stored in this inventory. If left at 0, there will be no cap.")]
        [SerializeField, Min(0)] private int _maxItemCount = 0;
        [Tooltip("Where to spawn items from this inventory.")]
        [SerializeField] private Transform _spawnPoint;
        [Tooltip("Where to place the menu once it has been opened.")]
        [SerializeField] private Transform _menuPosition;
        [Tooltip("If an animator is provided, it will have the bool 'Open' toggled while this menu is open.")]
        [SerializeField] private Animator _anim;
        [Tooltip("Items to be placed in this inventory initially.")]
        [SerializeField] private InventoryItem[] _startingItems;

        
        private VRCPlayerApi _localPlayer;
        private DataList _itemList = new DataList();
        private int _hashOpen = Animator.StringToHash("Open");
        private const int STORAGE_ID_NONE = 0;

        
        private void Start()
        {
            _localPlayer = Networking.LocalPlayer;

            foreach (InventoryItem item in _startingItems)
            {
                // Do not add this item if another player has already taken ownership of it.
                // Local items pass this check automatically.
                if (_localPlayer.IsOwner(item.gameObject)) 
                    _AddItem(item);
            }

            if (Library.LocalStorage == this)
                StorageId = _localPlayer.playerId;
        }
        
        /// <summary>
        /// Add an item from the storage.
        /// If a menu is bound, add the item to the menu as well.
        /// </summary>
        public void _AddItem(InventoryItem item, bool ignoreMenuCallback = false)
        {
            if (_itemList.Contains(item)) 
                return;
            
            _itemList.Add(item);
            
            if (Menu.CurrentInventory == this && !ignoreMenuCallback)
                Menu._AddItem(item);
            
            Networking.SetOwner(_localPlayer, item.gameObject);
            item._SetStorageId(StorageId);
            item._Hide();
        }

        /// <summary>
        /// Remove an item from the storage.
        /// If a menu is bound, remove the item from the menu as well.
        /// </summary>
        public void _RemoveItem(InventoryItem item, bool ignoreMenuCallback = false)
        {
            if (!_itemList.Contains(item)) 
                return;
            
            _itemList.Remove(item);

            if (Menu.CurrentInventory == this && !ignoreMenuCallback)
                Menu._RemoveItem(item);
            
            Networking.SetOwner(_localPlayer, item.gameObject);
            item._SetStorageId(STORAGE_ID_NONE);
        }

        /// <summary>
        /// Validate that all items are still in this inventory. If not, remove them.
        /// </summary>
        public void _ValidateItemList()
        {
            if (Library._ContainsAllItemsWithID(_itemList, StorageId))
                return;

            _BindAndOpenMenu();
        }

        public void _SpawnItem(InventoryItem item)
        {
            if (!item)
                return;
            item._Spawn(_spawnPoint);
        }

        /// <summary>
        /// Fill the ItemList will only items that contain this storage's ID.
        /// </summary>
        private void PopulateItems()
        {
            _itemList.Clear();
            _itemList = Library._GetItemsWithID(StorageId);
        }
        
        /// <summary>
        /// Bind this inventory to a given Menu and initialize it.
        /// </summary>
        public void _PopulateItemsAndOpenMenu()
        {
            PopulateItems();
            for (int i = 0; i < _itemList.Count; i++)
            {
                InventoryItem item = (InventoryItem)_itemList[i].Reference;
                Menu._AddItem(item, true);
            }
            
            OpenMenu();
        }

        /// <summary>
        /// Connect the menu to this storage inventory.
        /// This is meant to be called from Scene UI.
        /// </summary>
        [PublicAPI]
        public void _BindAndOpenMenu()
        {
            Menu._BindInventory(this);
            OpenMenu();
        }

        /// <summary>
        /// Open the menu at this position.
        /// </summary>
        private void OpenMenu()
        {
            Menu._Move(_menuPosition);
            Menu._ShowMenu();
            if (_anim) _anim.SetBool(_hashOpen, true);
        }

        public void _CloseMenu()
        {
            if (_anim) _anim.SetBool(_hashOpen, false);
        }
    }
}