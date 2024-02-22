using System.Collections;
using System.Collections.Generic;
using Infringed.Actions;
using UnityEngine;

namespace Infringed.InventorySystem
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Item Data", fileName = "New ItemData")]
    public class ItemData : ScriptableObject
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField, Multiline] public string Description { get; set; }
        [field: SerializeField] public Sprite Sprite { get; set; }
        [field: SerializeField] public GameObject Prefab { get; set; }
        [field: SerializeField] public ItemAction Action { get; set; }
    }
}
