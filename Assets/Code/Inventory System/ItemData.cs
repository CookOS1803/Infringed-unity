using System.Collections;
using System.Collections.Generic;
using Infringed.Actions;
using UnityEngine;
using Deadcows;

namespace Infringed.InventorySystem
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Item Data", fileName = "New ItemData")]
    public class ItemData : ScriptableObject
    {
        public enum ItemType { Inventory, Skill, Important }

        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, Multiline] public string Description { get; private set; }
        [field: SerializeField] public ItemType Type { get; private set; }
        [field: SerializeField] public Sprite BeltSprite { get; private set; }

        [field: SerializeField, ConditionalField(useMethod: true, nameof(IsInventoryItem))]
        public Sprite InventorySprite { get; private set; }

        [field: SerializeField, ConditionalField(useMethod: true, nameof(IsInventoryItem))]
        public GameObject Prefab { get; private set; }

        [field: SerializeField, ConditionalField(useMethod: true, nameof(IsImportantItem), inverse: true)]
        public ItemAction Action { get; private set; }
        
        [field: SerializeField, ConditionalField(useMethod: true, nameof(IsInventoryItem))]
        public int Width { get; private set; } = 1;
        
        [field: SerializeField, ConditionalField(useMethod: true, nameof(IsInventoryItem))]
        public int Height { get; private set; } = 1;

        [field: SerializeField, ConditionalField(useMethod: true, nameof(IsInventoryItem))]
        public bool Consumable { get; private set; } = true;

        public bool IsInventoryItem()
        {
            return Type == ItemType.Inventory;
        }

        public bool IsSkill()
        {
            return Type == ItemType.Skill;
        }

        public bool IsImportantItem()
        {
            return Type == ItemType.Important;
        }
    }
}
