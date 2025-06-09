using UdonSharp;
using UnityEngine;
using Vowgan.Inventory.UI;

namespace Vowgan.Inventory
{
    /// <summary>
    /// Base script class used by all Inventory components.
    /// These values are populated during PostProcessScene automation.
    /// </summary>
    public abstract class BaseInventoryBehaviour : UdonSharpBehaviour
    {
        [HideInInspector] public PickupInventoryMenu Menu;
        [HideInInspector] public PickupItemLibrary Library;
    }
}