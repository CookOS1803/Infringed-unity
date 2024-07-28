using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Infringed.InventorySystem.UI
{
    public class UIItemBackground : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField, Range(0f, 1f)] private float _normalAlpha = 0f;
        [SerializeField, Range(0f, 1f)] private float _hoverAlpha = 0.5f;
        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void OnEnable()
        {
            _SetAlpha(_normalAlpha);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!eventData.dragging)
                _SetAlpha(_hoverAlpha);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _SetAlpha(_normalAlpha);
        }

        private void _SetAlpha(float alpha)
        {
            var color = _image.color;
            color.a = alpha;
            _image.color = color;
        }
    }
}
