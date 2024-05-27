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
        [SerializeField, Range(0f, 1f)] private float _emptyItemAlpha = 0.5f;
        private RawImage _selectedSlotImage;

        public Belt Belt => _player.Belt;

        private void Start()
        {
            _RefreshBelt();
            Belt.OnChange += _RefreshBelt;
            Belt.OnSlotSelection += _SelectSlot;
            Belt.Inventory.OnItemAdd += _RefreshBeltWithAddedItem;
            Belt.Inventory.OnItemRemove += _RefreshBeltWithItem;

            for (int i = 0; i < Belt.Size; i++)
            {
                var child = transform.GetChild(i);
                child.GetComponent<BeltSlot>().Index = i;
                child.GetComponent<RawImage>().color = _defaultSlotColor;
            }

            _selectedSlotImage = transform.GetChild(Belt.SelectedSlot).GetComponent<RawImage>();
            _selectedSlotImage.color = _selectedSlotColor;
        }

        private void OnDestroy()
        {
            Belt.OnChange -= _RefreshBelt;
            Belt.OnSlotSelection -= _SelectSlot;
            Belt.Inventory.OnItemAdd -= _RefreshBeltWithAddedItem;
            Belt.Inventory.OnItemRemove -= _RefreshBeltWithItem;
        }

        private void _RefreshBelt()
        {
            for (int i = 0; i < Belt.Size; i++)
            {
                var slot = transform.GetChild(i).GetComponent<BeltSlot>();

                if (Belt[i] == null)
                {
                    slot.UnsetItem();
                }
                else
                {
                    slot.SetItem(Belt[i]);

                    var alpha = Belt.HasItemsInInventoty(i) ? 1f : _emptyItemAlpha;
                    slot.SetItemAlpha(alpha);
                }
            }
        }
        

        private void _RefreshBeltWithAddedItem(Item item)
        {
            if (!Belt.ContainsItem(item.Data))
            {
                var index = Belt.GetFirstNullIndex();

                if (index >= 0)
                {
                    Belt[index] = item.Data;
                    return;
                }
            }

            _RefreshBelt();
        }

        private void _RefreshBeltWithItem(Item item)
        {
            _RefreshBelt();
        }

        private void _SelectSlot()
        {
            _selectedSlotImage.color = _defaultSlotColor;

            _selectedSlotImage = transform.GetChild(Belt.SelectedSlot).GetComponent<RawImage>();
            _selectedSlotImage.color = _selectedSlotColor;
        }
    }
}
