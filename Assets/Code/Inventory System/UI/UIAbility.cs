using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Infringed.InventorySystem.UI
{
    public class UIAbility : MonoBehaviour
    {
        [Serializable]
        public struct Info
        {
            public TMP_Text titleText;
            public TMP_Text descriptionText;
            public TMP_Text costLabelText;
            public TMP_Text costValueText;
            public ToggleGroup toggleGroup;
            public Button button;
            public SkillSet skillSet;
            public AbilitySet abilitySet;
            public TestEssentia testEssentia;
        }

        [SerializeField] private Ability _ability;
        [SerializeField] private Image _abilityImage;
        [SerializeField] private Image _learnedImage;
        private Info _info;

        private AbilityInstance AbilityInstance => _info.abilitySet.GetAbilityInstance(_ability);

        private void Awake()
        {
            _abilityImage.sprite = _ability.Sprite;
        }

        public void Initialize(Info info)
        {
            _info = info;
            GetComponent<Toggle>().group = info.toggleGroup;
        }

        public void UpdateDescription(bool status)
        {
            if (status)
            {
                _info.titleText.text = _ability.Name;
                _info.descriptionText.text = _ability.Description;
                if (AbilityInstance.IsLearned)
                {
                    _info.button.gameObject.SetActive(false);
                    _info.costLabelText.gameObject.SetActive(false);
                    _info.costValueText.gameObject.SetActive(false);
                }
                else
                {
                    _info.button.gameObject.SetActive(true);
                    _info.button.onClick.AddListener(_OnButtonClicked);

                    _info.costLabelText.gameObject.SetActive(true);
                    _info.costValueText.gameObject.SetActive(true);
                    _info.costValueText.text = _ability.Cost.ToString();

                    _UpdateButtonInteraction();
                    _info.testEssentia.OnChange += _UpdateButtonInteraction;
                }
            }
            else
            {
                _info.titleText.text = String.Empty;
                _info.descriptionText.text = String.Empty;
                _info.costLabelText.gameObject.SetActive(false);
                _info.costValueText.gameObject.SetActive(false);
                _info.button.onClick.RemoveListener(_OnButtonClicked);
                _info.button.gameObject.SetActive(false);
                _info.testEssentia.OnChange -= _UpdateButtonInteraction;
            }
        }

        private void _OnButtonClicked()
        {
            if (!_info.testEssentia.Consume(_ability.Cost))
                return;

            AbilityInstance.IsLearned = true;

            if (_ability.GrantedSkill != null)
            {
                var skill = new SkillInstance(_ability.GrantedSkill);
                skill.IsLearned = true;
                _info.skillSet.AddSkill(skill);
            }

            _info.button.onClick.RemoveListener(_OnButtonClicked);
            _info.button.gameObject.SetActive(false);
            _info.costLabelText.gameObject.SetActive(false);
            _info.costValueText.gameObject.SetActive(false);
            _learnedImage.gameObject.SetActive(true);
            _info.testEssentia.OnChange -= _UpdateButtonInteraction;
        }

        private void _UpdateButtonInteraction()
        {
            _info.button.interactable = _IsLearnable();
        }

        private bool _IsLearnable()
        {
            foreach (var ability in _ability.Parents)
            {
                if (!_info.abilitySet.GetAbilityInstance(ability).IsLearned)
                    return false;
            }

            return _ability.Cost <= _info.testEssentia.Essentia;
        }
    }
}
