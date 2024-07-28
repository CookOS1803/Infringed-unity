using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Infringed.UI
{
    public class Draggable : MonoBehaviour, IDragHandler
    {
        public RectTransform RectTransform => transform as RectTransform;
        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            RectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }
    }
}
