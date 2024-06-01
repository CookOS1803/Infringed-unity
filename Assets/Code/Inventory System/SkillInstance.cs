using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.InventorySystem
{
    public class SkillInstance : ItemInstance
    {
        public SkillInstance(ItemData data) : base(data)
        {
        }

        public bool IsLearned { get; set; }
    }
}
