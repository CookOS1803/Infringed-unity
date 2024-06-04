using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Infringed.UI
{
    public class SizeSetter : MonoBehaviour
    {
        public UnityEvent<float, float> OnSizeSet;

        public RectTransform RectTransform => transform as RectTransform;
        [SerializeField] private float _width; 
        [SerializeField] private float _height;
        [SerializeField] private Vector2 _additionalSize;
        [SerializeField] private Vector2Int _minSize;

        public void Set(bool status)
        {
            if (!status)
                return;

            Set();
        } 

        public void Set(float width, float height)
        {
            _width = width;
            _height = height;

            Set();
        }

        public void Set()
        {
            var size = RectTransform.sizeDelta;
            size.x = Mathf.Max(_width + _additionalSize.x, _minSize.x);
            size.y = Mathf.Max(_height + _additionalSize.y, _minSize.y);
            RectTransform.sizeDelta = size;

            OnSizeSet?.Invoke(_width, _height);
        }
    }
}
