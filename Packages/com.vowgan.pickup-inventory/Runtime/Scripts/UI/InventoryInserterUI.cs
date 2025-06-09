using UnityEngine;
using UnityEngine.UI;

namespace Vowgan.Inventory.UI
{
    public class InventoryInserterUI : InventoryInserter
    {
        
        [SerializeField] private Image _spriteImage;
        [SerializeField] private Color _idleColor;
        [SerializeField] private Color _droppingColor;
        
        public override void _Highlight(bool value)
        {
            base._Highlight(value);
            _spriteImage.color = value ? _droppingColor : _idleColor;
        }
    }
}