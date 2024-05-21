using System.Collections;
using System.Collections.Generic;
using Infringed.Actions;
using UnityEngine;

namespace Infringed.InventorySystem
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Item Data", fileName = "New ItemData")]
    public class ItemData : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, Multiline] public string Description { get; private set; }
        [field: SerializeField] public Sprite InventorySprite { get; private set; }
        [field: SerializeField] public Sprite BeltSprite { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
        [field: SerializeField] public ItemAction Action { get; private set; }
        [field: SerializeField, Min(1)] public int Width { get; private set; } = 1;
        [field: SerializeField, Min(1)] public int Height { get; private set; } = 1;
    }
}
