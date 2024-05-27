using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infringed.InventorySystem
{
    public class SkillSet : MonoBehaviour
    {
        public event Action<ItemData> OnSkillAdd;

        [SerializeField] private List<ItemData> _skillsList;
        private HashSet<ItemData> _skills;

        private void Awake()
        {
            _skills = _skillsList.ToHashSet();
            _skillsList.Clear();
            _skillsList = null;
        }

        private void Start()
        {
            foreach (var skill in _skills)
            {
                OnSkillAdd?.Invoke(skill);
            }
        }
    }
}
