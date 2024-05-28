using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infringed.Player;
using UnityEngine;

namespace Infringed.InventorySystem
{
    public class SkillSet : MonoBehaviour
    {
        public event Action<ItemData> OnSkillAdd;

        [SerializeField] private List<ItemData> _skillsList;
        private HashSet<ItemData> _skills;
        private PlayerController _player;

        private void Awake()
        {
            _player = GetComponent<PlayerController>();

            _skills = _skillsList.ToHashSet();
            _skillsList.Clear();
            _skillsList = null;
        }

        private void Start()
        {
            foreach (var skill in _skills)
            {
                OnSkillAdd?.Invoke(skill);

                var i = _player.Belt.GetFirstNullIndex();

                if (i >= 0)
                {
                    _player.Belt[i] = skill;
                }
            }
        }
    }
}
