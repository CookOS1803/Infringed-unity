using System.Collections;
using System.Collections.Generic;
using Infringed.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Infringed.InventorySystem.UI
{
    public class UIAbilityTree : MonoBehaviour
    {
        [SerializeField] private Transform _abilitiesParent;
        [SerializeField] private Transform _thiefAbilities;
        [SerializeField] private Transform _mageAbilities;
        [SerializeField] private UIAbility.Info _info;

        private void Awake()
        {
            if (_info.abilitySet == null)
                _info.abilitySet = FindObjectOfType<AbilitySet>();

            if (_info.skillSet == null)
                _info.skillSet = FindObjectOfType<SkillSet>();
            
            var origin = (Origin)PlayerPrefs.GetInt("Origin", 0);

            if (origin == Origin.Thief)
                _thiefAbilities.gameObject.SetActive(true);
            else if (origin == Origin.Mage)
                _mageAbilities.gameObject.SetActive(true);
        }

        private void Start()
        {
            var abilities = _abilitiesParent.GetComponentsInChildren<UIAbility>();

            foreach (var ability in abilities)
            {
                ability.Initialize(_info);
            }
        }
    }
}
