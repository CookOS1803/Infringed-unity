using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Infringed.UI
{
    public class ScaleAsGrid : MonoBehaviour
    {
        public RectTransform RectTransform => transform as RectTransform;
        [field: SerializeField] public Vector2 AdditionalSize { get; private set; }
        [SerializeField] private Vector2Int _minSize;
        [SerializeField] private GridLayoutGroup _grid;

        private void Start()
        {
            SetSize();
        }

        public void SetSize(bool status)
        {
            if (status)
                SetSize();
        }

        public void SetSize()
        {
            var count = _grid.transform.childCount;

            var width = _grid.constraintCount;
            var height = count / width;

            var size = RectTransform.sizeDelta;
            var x = width * _grid.cellSize.x + (width - 1) * _grid.spacing.x + AdditionalSize.x;
            var y = height * _grid.cellSize.y + (height - 1) * _grid.spacing.y + AdditionalSize.y;
            size.x = Mathf.Max(x, _minSize.x);
            size.y = Mathf.Max(y, _minSize.y);
            RectTransform.sizeDelta = size;
        }
    }
}
