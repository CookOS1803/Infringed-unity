using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item Data", fileName = "New ItemData")]
public class ItemData : ScriptableObject
{
    [field: SerializeField] public new string name { get; set; }
    [field: SerializeField, Multiline] public string description { get; set; }
    [field: SerializeField] public Sprite sprite { get; set; }
    [field: SerializeField] public GameObject prefab { get; set; }
    [field: SerializeField] public ItemAction action { get; set; }
}
