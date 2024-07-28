using System.Collections;
using System.Collections.Generic;
using Infringed.Math;
using UnityEngine;
using UnityEngine.UI;

namespace Infringed.InventorySystem.UI
{
    public class ItemGhost : MonoBehaviour
    {
        public RectTransform RectTransform => transform as RectTransform;
        public Rectangle Rectangle { get; set; }
        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void Clone(UIItem item)
        {
            _image.sprite = item.Image.sprite;
            RectTransform.anchoredPosition = item.RectTransform.anchoredPosition;
            RectTransform.rotation = item.RectTransform.rotation;
            RectTransform.sizeDelta = item.RectTransform.sizeDelta;
        }
    }
}
