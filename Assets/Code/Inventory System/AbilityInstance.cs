using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.InventorySystem
{
    [System.Serializable]
    public class AbilityInstance
    {
        public event Action<AbilityInstance> OnLearned;

        public Ability Ability { get; private set; }
        public bool IsLearned
        {
            get => _isLearned;
            set
            {
                if (value && !_isLearned)
                {
                    _isLearned = value;
                    OnLearned?.Invoke(this);
                }
                else
                {
                    _isLearned = value;
                }
            }
        }
        private bool _isLearned;

        public AbilityInstance(Ability ability)
        {
            Ability = ability;
        }
    }
}
