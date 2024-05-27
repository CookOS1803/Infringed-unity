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
        [SerializeField] private GridLayoutGroup _grid;

        private void Start()
        {
            var count = _grid.transform.childCount;

            var width = _grid.constraintCount;
            var height = count / width;

            var size = RectTransform.sizeDelta;
            size.x = width * _grid.cellSize.x + (width - 1) * _grid.spacing.x + AdditionalSize.x;
            size.y = height * _grid.cellSize.y + (height - 1) * _grid.spacing.y + AdditionalSize.y;
            RectTransform.sizeDelta = size;
        }
    }
}
