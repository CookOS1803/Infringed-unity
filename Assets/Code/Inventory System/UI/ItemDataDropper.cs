using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Infringed.InventorySystem.UI
{
    public abstract class ItemDataDropper : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public RectTransform RectTransform => transform as RectTransform;
        public abstract ItemInstance DroppedItem { get; }
        protected Canvas _canvas;
        protected Vector2 _beforeDragPosition;
        protected Quaternion _beforeDragRotation;

        protected virtual void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            _beforeDragPosition = RectTransform.anchoredPosition;
            _beforeDragRotation = RectTransform.rotation;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            RectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            RectTransform.rotation = _beforeDragRotation;
            RectTransform.anchoredPosition = _beforeDragPosition;
        }
    }
}
