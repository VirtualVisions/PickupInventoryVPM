
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace Vowgan.Inventory
{
    /// <summary>
    /// Opens the assigned Storage inside the Menu.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class InventoryOpenInteract : BaseInventoryBehaviour
    {
        [SerializeField] private PickupInventoryStorage _inventory;
        public override void Interact() => _OnClick();
        
        /// <summary>
        /// Open the current storage in the menu.
        /// Available for running via UI buttons.
        /// </summary>
        [PublicAPI]
        public void _OnClick() => _inventory._BindAndOpenMenu();
    }
}