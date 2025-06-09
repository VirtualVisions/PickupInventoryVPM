using UdonSharp;
using UnityEngine;

namespace Vowgan.Inventory
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MenuSound : BaseInventoryBehaviour
    {
        
        [SerializeField] private AudioSource _source;
        
        [SerializeField] private AudioClip _clipOpenMenu;
        public void _PlayOpenMenu() => PlayClip(_clipOpenMenu);
        
        [SerializeField] private AudioClip _clipClickElement;
        public void _PlayClickElement() => PlayClip(_clipClickElement);
        
        [SerializeField] private AudioClip _clipSpawnItem;
        public void _PlaySpawnItem() => PlayClip(_clipSpawnItem);
        
        [SerializeField] private AudioClip _clipNotice;
        public void _PlayNotice() => PlayClip(_clipNotice);
        
        [SerializeField] private AudioClip _clipInsertActive;
        public void _PlayInsertActive() => PlayClip(_clipInsertActive);
        
        [SerializeField] private AudioClip _clipInsertInactive;
        public void _PlayInsertInactive() => PlayClip(_clipInsertInactive);
        
        [SerializeField] private AudioClip _clipInsertComplete;
        public void _PlayInsertComplete() => PlayClip(_clipInsertComplete);

        private void PlayClip(AudioClip clip)
        {
            _source.clip = clip;
            _source.Play();
        }

    }
}