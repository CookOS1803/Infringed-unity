using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.InventorySystem
{    
    [CreateAssetMenu(menuName = "Scriptable Objects/Ability", fileName = "New Ability")]
    public class Ability : ScriptableObject
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField, TextArea] public string Description { get; private set; }
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public int Cost { get; private set; }
        [field: SerializeField] public List<Ability> Parents { get; private set; }
        [field: SerializeField] public ItemData GrantedSkill { get; private set; }
    }
}
