
using UdonSharp;

namespace Vowgan.Inventory
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VendingButton : UdonSharpBehaviour
    {

        public VendingMachine VendingMachine;

        public override void Interact()
        {
            VendingMachine._VendNewItem();
        }
    }
}
