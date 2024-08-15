using UnityEngine;
using UnityEngine.UI;

namespace Vowgan.Inventory
{
    public class InventoryInserterUI : InventoryInserter
    {

        public Image SpriteImage;
        
        public Color IdleColor;
        public Color DroppingColor;

        
        public override void _Highlight(bool value)
        {
            base._Highlight(value);
            
            SpriteImage.color = value ? DroppingColor : IdleColor;
        }
    }
}