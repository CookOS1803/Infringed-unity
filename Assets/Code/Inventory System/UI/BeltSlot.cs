using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Infringed.InventorySystem.UI
{
    public class BeltSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
    {
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
        }

        public void SetItem(ItemData item)
        {
            _child.SetItem(item);
        }

        public void SetItemAlpha(float alpha)
        {
            var color = _child.Image.color;
            color.a = alpha;
            _child.Image.color = color;
        }

        public void OnDrop(PointerEventData eventData)
        {
            var inventoryItem = eventData.pointerDrag.GetComponent<UIItem>();

            if (inventoryItem != null)
            {
                _OnInventoryItem(inventoryItem);
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

        private void _OnInventoryItem(UIItem inventoryItem)
        {
            _uiBelt.Belt[Index] = inventoryItem.Item.Data;
        }

        private void _OnBeltItem(UIBeltItem beltItem)
        {
            beltItem.transform.SetParent(beltItem.Parent);
            _uiBelt.Belt.SwapSlots(Index, beltItem.Index);
        }
    }
}
