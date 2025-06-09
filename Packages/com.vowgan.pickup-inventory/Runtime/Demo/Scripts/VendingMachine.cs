using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace Vowgan.Inventory.Demo
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VendingMachine : UdonSharpBehaviour
    {
        [SerializeField] private Transform _spawnLocation;
        [SerializeField] private VRCObjectPool _pool;
        [SerializeField] private AudioSource _soundEffects;
        [SerializeField] private AudioClip _vendSuccess;

        private VRCPlayerApi _localPlayer;


        private void Start()
        {
            _localPlayer = Networking.LocalPlayer;
        }

        public void _VendNewItem()
        {
            Networking.SetOwner(_localPlayer, _pool.gameObject);
            GameObject itemObject = _pool.TryToSpawn();
            if (!itemObject) return;
            
            Networking.SetOwner(_localPlayer, itemObject.gameObject);
            InventoryItem item = itemObject.GetComponent<InventoryItem>();
            if (!item) return;
            
            item.Pickup.transform.SetPositionAndRotation(_spawnLocation.position, _spawnLocation.rotation);
            _soundEffects.PlayOneShot(_vendSuccess);
        }
    }
}