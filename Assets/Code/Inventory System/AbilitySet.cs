using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Deadcows;
using UnityEngine;

namespace Infringed.InventorySystem
{
    public class AbilitySet : MonoBehaviour
    {
        public event Action<AbilityInstance> OnLearned;

        private Dictionary<Ability, AbilityInstance> _abilities;

        private void Awake()
        {
            var list = Resources.LoadAll<Ability>("Abilities");

            _abilities = list.Select(ability =>
                             {
                                 var a = new AbilityInstance(ability);
                                 a.OnLearned += _OnLearned;
                                 return a;
                             })
                             .ToDictionary(inst => inst.Ability, inst => inst);
        }

        private void OnDestroy()
        {
            _abilities.ForEach(kv => kv.Value.OnLearned -= _OnLearned);
        }

        public AbilityInstance GetAbilityInstance(Ability key)
        {
            if (key != null && _abilities.TryGetValue(key, out var ability))
                return ability;

            return null;
        }

        private void _OnLearned(AbilityInstance instance)
        {
            instance.OnLearned -= _OnLearned;

            OnLearned?.Invoke(instance);
        }
    }
}
