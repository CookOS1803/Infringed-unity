using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Infringed.InventorySystem.UI
{
    public class UISkill : ItemDataDropper
    {
        public override ItemData DroppedData => _skill;
        public Transform OnDragParent { get; set; }
        private ItemData _skill;
        private Transform _initialParent;

        protected override void Awake()
        {
            base.Awake();

            _initialParent = transform.parent;
        }

        public void SetSkill(ItemData data)
        {
            _skill = data;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                base.OnBeginDrag(eventData);                
                transform.SetParent(OnDragParent);
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                base.OnDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                transform.SetParent(_initialParent);
                base.OnEndDrag(eventData);
            }
        }
    }
}
