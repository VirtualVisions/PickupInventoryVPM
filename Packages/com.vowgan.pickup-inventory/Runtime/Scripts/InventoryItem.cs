using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;

namespace Vowgan.Inventory
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class InventoryItem : BaseInventoryBehaviour
    {
        public VRCPickup Pickup => _pickup;
        public float StoredTimestamp => _storedTimestamp;
        
        public Sprite Icon => _icon;
        public string ItemName => _itemName;
        public string ItemDescription => _itemDescription;
        public InventoryItemSize ItemSize => _itemSize;
        
        [Header("Item Settings")]
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _itemName;
        [SerializeField] private string _itemDescription;
        [SerializeField] private InventoryItemSize _itemSize = InventoryItemSize.Medium;

        [Header("References")]
        [SerializeField] protected VRCPickup _pickup;

        [Header("Readout")]
        [ReadoutOnly] public bool _justSpawned;
        [SerializeField, ReadoutOnly] protected float _storedTimestamp;

        protected GameObject _pickupObj;
        protected Rigidbody _rigidbody;
        protected bool _startsKinematic;
        protected bool _initialized;

        
        
        [SerializeField, ReadoutOnly] protected int _storageId = 0;
        /// <summary>
        /// Get the ID of the inventory storage currently associated with this item.
        /// 0 means that none are associated. Positive values are player inventories. Negative values are world inventories.
        /// </summary>
        public virtual int _GetStorageId() => _storageId;
        public virtual void _SetStorageId(int id)
        {
            _storageId = id;
        }

        /// <summary>
        /// Get the string name of the current size.
        /// </summary>
        public string _GetSizeName() => InventoryUtility._ItemSizeName(_itemSize);

        private void Start()
        {
            if (!_initialized) _Init();
        }

        protected virtual void _Init()
        {
            _initialized = true;
            _rigidbody = _pickup.GetComponent<Rigidbody>();
            _startsKinematic = _rigidbody.isKinematic;
            _pickupObj = _pickup.gameObject;
        }

        /// <summary>
        /// Spawn this item at a given point, freezing it kinematically.
        /// </summary>
        public virtual void _Spawn(Transform point)
        {
            if (!_initialized) _Init();
            
            _justSpawned = true;
            _pickup.transform.SetPositionAndRotation(point.position, point.rotation);
            _pickupObj.SetActive(true);
            _rigidbody.isKinematic = true;
        }

        /// <summary>
        /// Hide this item after being stored into an inventory storage.
        /// </summary>
        public virtual void _Hide()
        {
            if (!_initialized) _Init();
            
            _pickup.Drop();
            _pickupObj.SetActive(false);
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _storedTimestamp = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// Clear the temporary kinematic state assigned from spawning.
        /// </summary>
        public virtual void _RunFirstPickupAfterSpawn()
        {
            _rigidbody.isKinematic = _startsKinematic;
        }
    }
}