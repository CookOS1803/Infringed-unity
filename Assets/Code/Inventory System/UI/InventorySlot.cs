using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.InventorySystem.UI
{
    public class InventorySlot : MonoBehaviour
    {
        public RectTransform RectTransform => transform as RectTransform;
        public Vector2Int GridPosition { get; set; }
    }
}
