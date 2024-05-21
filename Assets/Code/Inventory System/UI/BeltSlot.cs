using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Infringed.InventorySystem.UI
{
    public class BeltSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
    {
        [Inject] private UIBelt _uiInventory;
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

        public void OnDrop(PointerEventData eventData)
        {
            var item = eventData.pointerDrag.GetComponent<UIBeltItem>();

            item.transform.SetParent(item.Parent);
            _uiInventory.Belt.SwapSlots(Index, item.Index);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _uiInventory.Belt.SelectedSlot = Index;
        }
    }
}
