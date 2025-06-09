using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace Vowgan.Inventory
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class InventoryItemSynced : InventoryItem
    {

        [Header("Synced Data")] 
        [UdonSynced, SerializeField, ReadoutOnly] protected int _syncedStorageId;

        private VRCObjectSync _objectSync;

        public override int _GetStorageId() => _syncedStorageId;
        public override void _SetStorageId(int id)
        {
            base._SetStorageId(id);
            _syncedStorageId = id;
            RequestSerialization();
            OnDeserialization();
        } 
        
        protected override void _Init()
        {
            base._Init();
            _objectSync = _pickup.GetComponent<VRCObjectSync>();
        }

        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            // Only respawn the item if it was in a player inventory when the owner left.
            if (_syncedStorageId <= 0) return;
            
            if (player.isLocal && player.isMaster)
            {
                _Spawn(_pickup.transform);
                _objectSync.Respawn();
            }
        }

        public override void OnDeserialization()
        {
            // Send a checkup one frame later to validate for non-owners.
            if (Menu.CurrentInventory) Menu.CurrentInventory.SendCustomEventDelayedFrames(
                    nameof(PickupInventoryStorage._ValidateItemList), 1);
            
            _pickup.gameObject.SetActive(_syncedStorageId == 0);
            if (_syncedStorageId != 0) _pickup.Drop();
        }
        
        public override void _Spawn(Transform point)
        {
            base._Spawn(point);
            
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            Networking.SetOwner(Networking.LocalPlayer, _objectSync.gameObject);
            
            _syncedStorageId = 0;
            _objectSync.SetKinematic(true);
            _objectSync.FlagDiscontinuity();
            RequestSerialization();
        }

        public override void _RunFirstPickupAfterSpawn()
        {
            base._RunFirstPickupAfterSpawn();
            _objectSync.SetKinematic(_startsKinematic);
        }
    }
}