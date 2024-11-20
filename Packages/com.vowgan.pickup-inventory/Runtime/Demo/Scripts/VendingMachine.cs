using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace Vowgan.Inventory
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VendingMachine : UdonSharpBehaviour
    {
        public Transform SpawnLocation;
        public VRCObjectPool Pool;
        public AudioSource SoundEffects;
        public AudioClip VendSuccess;

        private VRCPlayerApi m_localPlayer;


        private void Start()
        {
            m_localPlayer = Networking.LocalPlayer;
        }

        public void _VendNewItem()
        {
            Networking.SetOwner(m_localPlayer, Pool.gameObject);
            GameObject itemObject = Pool.TryToSpawn();
            if (!itemObject) return;
            
            InventoryItem item = itemObject.GetComponent<InventoryItem>();
            if (!item) return;
            
            item.Pickup.transform.SetPositionAndRotation(SpawnLocation.position, SpawnLocation.rotation);
            SoundEffects.PlayOneShot(VendSuccess);
        }
    }
}