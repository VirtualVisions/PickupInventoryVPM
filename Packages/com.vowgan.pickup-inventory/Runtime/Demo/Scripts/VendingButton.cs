
using UdonSharp;
using UnityEngine;

namespace Vowgan.Inventory.Demo
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VendingButton : UdonSharpBehaviour
    {

        [SerializeField] private VendingMachine _vendingMachine;

        public override void Interact()
        {
            _vendingMachine._VendNewItem();
        }
    }
}
