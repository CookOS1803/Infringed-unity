using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.InventorySystem
{
    public class ItemInstance
    {
        public ItemData Data { get; protected set; }        
        public float LastUsedTime { get; set; }
        public float Cooldown { get; set; }

        public float CurrentCooldown
        {
            get
            {
                var delta = (LastUsedTime + Cooldown) - Time.time;

                if (delta < 0)
                    return 0;

                return delta;
            }
        }

        public ItemInstance(ItemData data)
        {
            Data = data;
            Cooldown = Data.Cooldown;
        }
    }
}
