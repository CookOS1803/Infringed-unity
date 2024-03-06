using System.Collections;
using System.Collections.Generic;
using Infringed.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Infringed.InventorySystem.UI
{
    public class UIInventory : MonoBehaviour
    {
        [SerializeField] private PlayerController _player;
        [SerializeField] private Color _defaultSlotColor;
        [SerializeField] private Color _selectedSlotColor;
        private RawImage _selectedSlotImage;

        public Inventory Inventory => _player.Inventory;

        private void Start()
        {
            _RefreshInventory();
            Inventory.OnChange += _RefreshInventory;
            Inventory.OnSlotSelection += _SelectSlot;

            for (int i = 0; i < Inventory.Size; i++)
            {
                Transform child = transform.GetChild(i);
                child.GetComponent<ItemSlot>().Index = i;
                child.GetComponent<RawImage>().color = _defaultSlotColor;
            }

            _selectedSlotImage = transform.GetChild(Inventory.SelectedSlot).GetComponent<RawImage>();
            _selectedSlotImage.color = _selectedSlotColor;
        }

        private void _RefreshInventory()
        {
            for (int i = 0; i < Inventory.Size; i++)
            {
                ItemSlot slot = transform.GetChild(i).GetComponent<ItemSlot>();

                if (Inventory[i] == null)
                {
                    slot.UnsetItem();
                }
                else
                {
                    slot.SetItem(Inventory[i].Data);
                }
            }
        }

        private void _SelectSlot()
        {
            _selectedSlotImage.color = _defaultSlotColor;

            _selectedSlotImage = transform.GetChild(Inventory.SelectedSlot).GetComponent<RawImage>();
            _selectedSlotImage.color = _selectedSlotColor;
        }
    }
}
