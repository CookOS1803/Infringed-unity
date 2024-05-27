using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Infringed.InventorySystem.UI
{
    public class BeltSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
    {
        [SerializeField] private TMP_Text _itemCountText;
        [Inject] private UIBelt _uiBelt;
        private UIBeltItem _child;
        private Vector2 _initialPosition;
        public int Index { get; set; }

        private void Awake()
        {
            _child = GetComponentInChildren<UIBeltItem>();
        }

        public void UnsetItem()
        {
            _child.UnsetItem();
            _itemCountText.enabled = false;
        }

        public void SetItem(ItemData data)
        {
            _child.SetItem(data);

            if (data.IsInventoryItem() && data.Consumable)
            {
                var count = _uiBelt.Belt.Inventory.GetItemCount(data);

                _itemCountText.enabled = count > 0;
                _itemCountText.text = count.ToString();
            }
            else
            {
                _itemCountText.enabled = false;
            }
        }

        public void SetItemAlpha(float alpha)
        {
            var color = _child.Image.color;
            color.a = alpha;
            _child.Image.color = color;
        }

        public void OnDrop(PointerEventData eventData)
        {
            var dropper = eventData.pointerDrag.GetComponent<ItemDataDropper>();

            if (dropper != null)
            {
                _OnDropper(dropper);
                return;
            }

            var beltItem = eventData.pointerDrag.GetComponent<UIBeltItem>();

            if (beltItem != null)
                _OnBeltItem(beltItem);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                _uiBelt.Belt.SelectedSlot = Index;
        }

        private void _OnDropper(ItemDataDropper dropper)
        {
            _uiBelt.Belt[Index] = dropper.DroppedData;
        }

        private void _OnBeltItem(UIBeltItem beltItem)
        {
            beltItem.transform.SetParent(beltItem.Parent);
            _uiBelt.Belt.SwapSlots(Index, beltItem.Index);
        }
    }
}
