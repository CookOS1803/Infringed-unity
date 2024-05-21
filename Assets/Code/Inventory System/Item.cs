using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.InventorySystem
{
    public class Item
    {
        public ItemData Data { get; private set; }
        public Vector2Int BottomLeftPosition { get; set; }
        public Vector2Int TopRightPosition { get; set; }

        public Item(ItemData newData)
        {
            Data = newData;
        }

        public bool IsRotated()
        {
            var dx = TopRightPosition.x - BottomLeftPosition.x;

            return Data.Width != dx + 1;
        }
    }
}
