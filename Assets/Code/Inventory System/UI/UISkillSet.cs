using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.InventorySystem.UI
{
    public class UISkillSet : MonoBehaviour
    {
        [field: SerializeField] public SkillSet SkillSet { get; private set; }
        [SerializeField] private GameObject _skillPrefab;
        [SerializeField] private Transform _skillsParent;

        private void Awake()
        {
            if (SkillSet == null)
                SkillSet = FindObjectOfType<SkillSet>();

            SkillSet.OnSkillAdd += _OnSkillAdd;
        }

        private void OnDestroy()
        {
            SkillSet.OnSkillAdd -= _OnSkillAdd;
        }

        private void _OnSkillAdd(ItemData data)
        {
            var instance = Instantiate(_skillPrefab, _skillsParent).GetComponent<UISkillSlot>();
            instance.InitializeSkill(data, transform);
        }
    }
}
