using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.InventorySystem
{
    public class Item
    {
        public ItemData Data { get; private set; }

        public Item(ItemData newData)
        {
            Data = newData;
        }
    }
}
