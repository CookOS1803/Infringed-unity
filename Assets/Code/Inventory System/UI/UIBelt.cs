using System.Collections;
using System.Collections.Generic;
using Infringed.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Infringed.InventorySystem.UI
{
    public class UIBelt : MonoBehaviour
    {
        [SerializeField] private PlayerController _player;
        [SerializeField] private Color _defaultSlotColor;
        [SerializeField] private Color _selectedSlotColor;
        private RawImage _selectedSlotImage;

        public Belt Belt => _player.Belt;

        private void Start()
        {
            _RefreshBelt();
            Belt.OnChange += _RefreshBelt;
            Belt.OnSlotSelection += _SelectSlot;

            for (int i = 0; i < Belt.Size; i++)
            {
                Transform child = transform.GetChild(i);
                child.GetComponent<BeltSlot>().Index = i;
                child.GetComponent<RawImage>().color = _defaultSlotColor;
            }

            _selectedSlotImage = transform.GetChild(Belt.SelectedSlot).GetComponent<RawImage>();
            _selectedSlotImage.color = _selectedSlotColor;
        }

        private void _RefreshBelt()
        {
            for (int i = 0; i < Belt.Size; i++)
            {
                BeltSlot slot = transform.GetChild(i).GetComponent<BeltSlot>();

                if (Belt[i] == null)
                {
                    slot.UnsetItem();
                }
                else
                {
                    slot.SetItem(Belt[i].Data);
                }
            }
        }

        private void _SelectSlot()
        {
            _selectedSlotImage.color = _defaultSlotColor;

            _selectedSlotImage = transform.GetChild(Belt.SelectedSlot).GetComponent<RawImage>();
            _selectedSlotImage.color = _selectedSlotColor;
        }
    }
}
