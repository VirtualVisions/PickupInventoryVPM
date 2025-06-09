
using System;
using JetBrains.Annotations;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace Vowgan.Inventory.UI
{

    public enum SortingMethod
    {
        Latest,
        Az,
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PickupInventoryMenu : BaseInventoryBehaviour
    {

        public PickupInventoryStorage CurrentInventory => _currentInventory;
        public MenuModal Modal => _modal;
        public MenuSound Sound => _sound;

        [SerializeField] private SortingMethod _sorting;
        [SerializeField] private float _spawnInsteadOfCloseDistance = 1;
        [SerializeField] private float _hideDistance = 5;

        [Header("References")] [SerializeField]
        private GameObject _buttonPrefab;

        [SerializeField] private Transform _buttonParent;
        [SerializeField] private MeshRenderer _canvasVisibleChecker;
        [SerializeField] private InventoryInserter _inserter;
        [SerializeField] private MenuModal _modal;
        [SerializeField] private MenuSound _sound;

        [Header("UI")] [SerializeField] private GameObject _menuContainer;
        [SerializeField] private TMP_InputField _searchingField;
        [SerializeField] private TMP_Dropdown _sortingDropdown;
        [SerializeField] private TextMeshProUGUI _maxSizeLabel;
        [SerializeField] private TextMeshProUGUI _totalItemCountLabel;
        [SerializeField] private TextMeshProUGUI _inventoryNameLabel;
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TextMeshProUGUI _itemName;
        [SerializeField] private TextMeshProUGUI _itemSizeLabel;
        [SerializeField] private TextMeshProUGUI _itemDescription;
        [SerializeField] private Button _spawnButton;

        [Header("Debug Readout")] 
        [SerializeField, ReadoutOnly] private InventoryItem _selectedItem;
        [SerializeField, ReadoutOnly] private PickupInventoryStorage _currentInventory;

        private VRCPlayerApi _localPlayer;
        private DataDictionary _itemButtons = new DataDictionary();
        private Transform _menuContainerTrans;
        private Transform _localStorageTrans;
        
        private float _lastVerticalInput;
        private float _verticalLimitForMenu = 0.5f;
        private float _nextTimeDistanceCheck;


        private void Start()
        {
            _localPlayer = Networking.LocalPlayer;
            _menuContainerTrans = _menuContainer.transform;
            _localStorageTrans = Library.LocalStorage.transform;
            ClearMenuSelection();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
                ToggleMenu();

            if (Time.realtimeSinceStartup >= _nextTimeDistanceCheck)
            {
                _nextTimeDistanceCheck += 2;

                if (Vector3.Distance(_localPlayer.GetPosition(), _menuContainerTrans.position) > _hideDistance)
                {
                    if (!_canvasVisibleChecker.isVisible)
                        _CloseMenu();
                }
            }
        }

        /// <summary>
        /// Toggle the state of the menu, or respawn it if the user is close to it.
        /// </summary>
        private void ToggleMenu()
        {
            if (_menuContainer.activeSelf)
            {
                if (Vector3.Distance(_localPlayer.GetPosition(), _localStorageTrans.position) > _spawnInsteadOfCloseDistance ||
                    _currentInventory != Library.LocalStorage)
                    SpawnMenuAtPlayer();
                else
                    _CloseMenu();
            }
            else
                SpawnMenuAtPlayer();
        }

        /// <summary>
        /// This is used when the VR player moves their right stick up and down, which is how we open the menu.
        /// </summary>
        public override void InputLookVertical(float value, UdonInputEventArgs args)
        {
            if (!_localPlayer.IsUserInVR()) return;

            if (_lastVerticalInput > _verticalLimitForMenu && value <= _verticalLimitForMenu)
                ToggleMenu();

            _lastVerticalInput = value;
        }

        /// <summary>
        /// Move the menu canvas to a target transform.
        /// </summary>
        public void _Move(Transform target)
        {
            _menuContainerTrans.SetPositionAndRotation(target.position, target.rotation);
        }

        /// <summary>
        /// Bind the menu's data and references to an existing inventory storage.
        /// </summary>
        public void _BindInventory(PickupInventoryStorage inventoryStorage)
        {
            ClearMenuItems();

            if (_currentInventory && _currentInventory != inventoryStorage)
            {
                _currentInventory._CloseMenu();
            }
            
            _currentInventory = inventoryStorage;
            _currentInventory._PopulateItemsAndOpenMenu();

            _inventoryNameLabel.text = _currentInventory.gameObject.name;
            _maxSizeLabel.text = $"Max Size: {InventoryUtility._ItemSizeName(_currentInventory.MaxItemSize)}";
        }

        /// <summary>
        /// Immediately display the menu.
        /// </summary>
        public void _ShowMenu()
        {
            // Scale to the user's size.
            _localStorageTrans.localScale = Vector3.one * _localPlayer.GetAvatarEyeHeightAsMeters();
            _menuContainer.SetActive(true);
            _sound._PlayOpenMenu();
            UpdateItemCountLabel();
        }
        
        /// <summary>
        /// This is called both internally and from the UI itself.
        /// </summary>
        [PublicAPI]
        public void _CloseMenu()
        {
            _menuContainer.SetActive(false);
        }

        /// <summary>
        /// Scale and move the menu in front of the player, binding to their local inventory.
        /// </summary>
        private void SpawnMenuAtPlayer()
        {
            // Select the local inventory when spawned at the player.
            _BindInventory(Library.LocalStorage);
            
            // Scale and move the menu.
            _localStorageTrans.position = _localPlayer.GetPosition();
            _localStorageTrans.rotation = _localPlayer.GetRotation();
        }

        /// <summary>
        /// Add an item to the list.
        /// This should only be used while the menu is already open.
        /// </summary>
        public void _AddItem(InventoryItem item, bool skipSelection = false)
        {
            GameObject buttonObj = Instantiate(_buttonPrefab, _buttonParent);
            buttonObj.name = $"{item.name} Button";

            _itemButtons[item] = buttonObj;

            ItemButtonUi button = buttonObj.GetComponent<ItemButtonUi>();
            button._Init(this, item);

            if (skipSelection) return;

            _inserter._Highlight(false);
            _SelectItem(item);
            _SortList();
            UpdateItemCountLabel();
            _sound._PlayInsertComplete();
        }

        /// <summary>
        /// Select an item to be previewed within the menu.
        /// </summary>
        public void _SelectItem(InventoryItem item)
        {
            _selectedItem = item;

            _itemIcon.sprite = item.Icon;
            _itemIcon.color = Color.white;
            _itemName.text = item.ItemName;
            _itemSizeLabel.text = $"Size Class: {item._GetSizeName()}";
            _itemDescription.text = item.ItemDescription;
            _spawnButton.interactable = true;
            
            _sound._PlayClickElement();
        }

        /// <summary>
        /// Called whenever the SearchField text is updated.
        /// </summary>
        [PublicAPI]
        public void _SearchForItems()
        {
            string searchText = _searchingField.text.ToLowerInvariant();

            if (searchText == string.Empty)
            {
                for (int i = 0; i < _buttonParent.childCount; i++)
                {
                    _buttonParent.GetChild(i).gameObject.SetActive(true);
                }

                return;
            }

            for (int i = 0; i < _buttonParent.childCount; i++)
            {
                GameObject child = _buttonParent.GetChild(i).gameObject;
                InventoryItem item = child.GetComponent<ItemButtonUi>().Item;

                child.SetActive(item.ItemName.ToLowerInvariant().Contains(searchText));
            }
        }

        /// <summary>
        /// Visually sort the menu's inventory list.
        /// </summary>
        public void _SortList()
        {
            int childCount = _buttonParent.childCount;

            for (int x = 0; x < childCount - 1; x++)
            {
                for (int y = 0; y < childCount - x - 1; y++)
                {
                    Transform child1 = _buttonParent.GetChild(y);
                    Transform child2 = _buttonParent.GetChild(y + 1);

                    InventoryItem item1 = child1.GetComponent<ItemButtonUi>().Item;
                    InventoryItem item2 = child2.GetComponent<ItemButtonUi>().Item;

                    switch (_sorting)
                    {
                        case SortingMethod.Az:
                            // Compare and swap if necessary
                            if (string.CompareOrdinal(item1.ItemName, item2.ItemName) > 0)
                            {
                                child1.SetSiblingIndex(y + 1);
                                child2.SetSiblingIndex(y);
                            }

                            break;

                        case SortingMethod.Latest:
                            if (item1.StoredTimestamp < item2.StoredTimestamp)
                            {
                                child1.SetSiblingIndex(y + 1);
                                child2.SetSiblingIndex(y);
                            }

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Declare a specific sorting method for the Menu.
        /// </summary>
        [PublicAPI]
        public void _SetSorting()
        {
            _sorting = (SortingMethod)_sortingDropdown.value;
            _SortList();
        }

        private void UpdateItemCountLabel()
        {
            string maxSizeLabel;
            if (_currentInventory.MaxItemCount == 0)
                maxSizeLabel = $"Total Items: {_currentInventory.ItemCount}";
            else
                maxSizeLabel = $"Total Items: {_currentInventory.ItemCount} / {_currentInventory.MaxItemCount}";
            _totalItemCountLabel.text = maxSizeLabel;
        }

        /// <summary>
        /// Clear out the current selected object in the menu.
        /// </summary>
        private void ClearMenuSelection()
        {
            _itemIcon.sprite = null;
            _itemIcon.color = Color.clear;
            _itemName.text = string.Empty;
            _itemSizeLabel.text = string.Empty;
            _itemDescription.text = string.Empty;
            _spawnButton.interactable = false;
        }

        /// <summary>
        /// Remove all buttons from the menu.
        /// </summary>
        private void ClearMenuItems()
        {
            DataList keys = _itemButtons.GetKeys();
            for (int i = 0; i < keys.Count; i++)
            {
                GameObject button = (GameObject)_itemButtons[keys[i]].Reference;
                Destroy(button);
            }
            _itemButtons.Clear();
            ClearMenuSelection();
        }

        /// <summary>
        /// Place an item from the menu, then remove it from the inventory.
        /// </summary>
        [PublicAPI]
        public void _SpawnItem()
        {
            if (!_selectedItem)
                return;
            
            _sound._PlaySpawnItem();
            _currentInventory._SpawnItem(_selectedItem);
            _RemoveItem(_selectedItem);
        }

        /// <summary>
        /// Removes an item from the ItemList, updating the UI accordingly.
        /// </summary>
        public void _RemoveItem(InventoryItem item)
        {
            // Check if we currently have an inventory.
            if (!_currentInventory)
            {
                Debug.LogWarning("No inventory currently selected to remove item from.", this);
                return;
            }
            
            int index = _currentInventory.ItemList.IndexOf(item);
            if (index == -1)
            {
                Debug.LogWarning($"Storage {_currentInventory} does not currently contain {item}", this);
                return;
            }

            GameObject button = (GameObject)_itemButtons[item].Reference;
            Destroy(button);

            _itemButtons.Remove(item); 
            _currentInventory._RemoveItem(_selectedItem, true);

            // Select a new item in the Menu.
            int newCount = _currentInventory.ItemList.Count;
            if (newCount == 0)
            {
                ClearMenuSelection();
                _selectedItem = null;
            }
            else
            {
                int clampedIndex = Mathf.Clamp(index, 0, newCount - 1);
                _SelectItem((InventoryItem)_currentInventory.ItemList[clampedIndex].Reference);
            }
            UpdateItemCountLabel();
        }
    }
}