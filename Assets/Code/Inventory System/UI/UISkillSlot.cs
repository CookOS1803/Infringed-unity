using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Infringed.InventorySystem.UI
{
    public class UISkillSlot : MonoBehaviour
    {
        [field: SerializeField] public Image SkillImage { get; private set; }
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Graphic[] _additionalGraphics;
        [SerializeField] private UISkill _skill;
        private Graphic _graphicToMatch;

        private void Awake()
        {
            _graphicToMatch = transform.parent.GetComponentInParent<Graphic>();
        }

        public void InitializeSkill(ItemData data, Transform onDragParent)
        {
            _title.text = data.Name;
            _description.text = data.Description;
            SkillImage.sprite = data.BeltSprite;
            _skill.SetSkill(data);
            _skill.OnDragParent = onDragParent;

            var status = _graphicToMatch.enabled;
            
            _title.enabled = status;
            _description.enabled = status;
            SkillImage.enabled = status;

            foreach (var g in _additionalGraphics)
            {
                g.enabled = status;
            }
        }
    }
}
