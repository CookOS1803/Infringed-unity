using System.Collections;
using System.Collections.Generic;
using Infringed.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Infringed.InventorySystem.UI
{
    public class UISkill : ItemDataDropper
    {
        public override ItemData DroppedData => _skill;
        public Transform OnDragParent { get; set; }
        private ItemData _skill;
        private Transform _initialParent;
        private Image _image;
        private TestEssentia essentia;

        protected override void Awake()
        {
            base.Awake();

            _initialParent = transform.parent;
            _image = GetComponent<Image>();
        }

        private void Start()
        {
            essentia = FindObjectOfType<TestEssentia>();
        }

        private void Update()
        {
            if (!_skill)
                return;

            var color = UnityEngine.Color.white;

            color.a = _skill.IsLearned ? 1 : 0.4f;

            _image.color = color;
        }

        public void SetSkill(ItemData data)
        {
            _skill = data;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (!_skill.IsLearned)
                return;

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                base.OnBeginDrag(eventData);
                transform.SetParent(OnDragParent);
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (!_skill.IsLearned)
                return;

            if (eventData.button == PointerEventData.InputButton.Left)
                base.OnDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (!_skill.IsLearned)
                return;

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                transform.SetParent(_initialParent);
                base.OnEndDrag(eventData);
            }
        }

        public void Learn()
        {
            if (essentia.Consume())
                _skill.IsLearned = true;
        }
    }
}
