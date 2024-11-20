using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;

namespace Vowgan.Inventory
{

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class InventoryItem : UdonSharpBehaviour
    {
        
        [Header("Item Settings")]
        public Sprite Icon;
        public string ItemName;
        public string ItemDescription;

        [Header("References")]
        public VRCPickup Pickup;
        public float StoredTimestamp;
        public bool JustSpawned;

        protected GameObject m_pickupObj;
        protected Rigidbody m_rigidbody;
        protected bool m_startsKinematic;
        protected bool m_initialized;
        
        
        private void Start()
        {
            if (!m_initialized) _Init();
        }

        protected virtual void _Init()
        {
            m_initialized = true;
            m_rigidbody = Pickup.GetComponent<Rigidbody>();
            m_startsKinematic = m_rigidbody.isKinematic;
            m_pickupObj = Pickup.gameObject;
        }

        public virtual void _Spawn(Transform point)
        {
            if (!m_initialized) _Init();
            
            JustSpawned = true;
            Pickup.transform.SetPositionAndRotation(point.position, point.rotation);
            m_pickupObj.SetActive(true);
            m_rigidbody.isKinematic = true;
        }

        public virtual void _Hide()
        {
            if (!m_initialized) _Init();
            
            Pickup.Drop();
            m_pickupObj.SetActive(false);
            m_rigidbody.velocity = Vector3.zero;
            m_rigidbody.angularVelocity = Vector3.zero;
            StoredTimestamp = Time.realtimeSinceStartup;
        }

        public virtual void _RunFirstPickupAfterSpawn()
        {
            m_rigidbody.isKinematic = m_startsKinematic;
        }
        
    }
}