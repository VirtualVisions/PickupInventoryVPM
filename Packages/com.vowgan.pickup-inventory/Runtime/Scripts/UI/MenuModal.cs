using JetBrains.Annotations;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Vowgan.Inventory.UI
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MenuModal : BaseInventoryBehaviour
    {

        public bool ModalActive => _modalActive;

        [SerializeField] private float _showDuration = 3;
        [SerializeField] private Image _progressFill;
        [SerializeField] private TextMeshProUGUI _titleLabel;
        [SerializeField] private TextMeshProUGUI _descriptionLabel;

        private bool _modalActive;
        private float _totalShowTime;

        private void OnDisable()
        {
            _ClearModal();
        }

        /// <summary>
        /// Display a popup modal that informs the user for a set amount of time.
        /// </summary>
        public void _ShowModal(string title, string description)
        {
            Menu.Sound._PlayNotice();
            _modalActive = true;
            _totalShowTime = 0;
            _titleLabel.text = title;
            _descriptionLabel.text = description;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Hide the currently active modal.
        /// Triggered both by timeout and by clicking to clear the modal.
        /// </summary>
        [PublicAPI]
        public void _ClearModal()
        {
            Menu.Sound._PlayClickElement();
            _modalActive = false;
            gameObject.SetActive(false);
        }
        
        private void Update()
        {
            _totalShowTime += Time.deltaTime;
            _progressFill.fillAmount = 1 - _totalShowTime / _showDuration;
            if (_totalShowTime >= _showDuration)
            {
                _ClearModal();
            }
        }
    }
}