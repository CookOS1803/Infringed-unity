using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Infringed.InventorySystem.UI
{
    public class ItemSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
    {
        [Inject] private UIInventory _uiInventory;
        private UIItem _child;
        private Vector2 _initialPosition;
        public int Index { get; set; }

        private void Awake()
        {
            _child = GetComponentInChildren<UIItem>();
            _child.Inventory = _uiInventory.Inventory;
        }

        public void UnsetItem()
        {
            _child.UnsetItem();
        }

        public void SetItem(ItemData item)
        {
            _child.SetItem(item);
        }

        public void OnDrop(PointerEventData eventData)
        {
            var item = eventData.pointerDrag.GetComponent<UIItem>();

            item.transform.SetParent(item.Parent);
            _uiInventory.Inventory.SwapSlots(Index, item.Index);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _uiInventory.Inventory.SelectedSlot = Index;
        }
    }
}
