using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Infringed.Math;

namespace Infringed.InventorySystem
{
    [System.Serializable]
    public class InventoryItemInstance : ItemInstance
    {
        public enum RotationStatus { NonRotated, Rotated, Square, Invalid }

        public Rectangle Rectangle { get; set; }

        public InventoryItemInstance(ItemData data) : base(data)
        {
        }

        public bool IsRotated()
        {
            var dx = Rectangle.DeltaX;

            return Data.Width != dx + 1;
        }

        public RotationStatus GetRotationStatus()
        {
            return GetRotationStatus(Rectangle);
        }

        public RotationStatus GetRotationStatus(Rectangle rectangle)
        {
            var width = rectangle.DeltaX + 1;
            var height = rectangle.DeltaY + 1;

            if (Data.Width == width && Data.Height == height)
            {
                if (width == height)
                    return RotationStatus.Square;

                return RotationStatus.NonRotated;
            }
            if (Data.Width == height && Data.Height == width)
                return RotationStatus.Rotated;
            
            return RotationStatus.Invalid;
        }
    }
}
