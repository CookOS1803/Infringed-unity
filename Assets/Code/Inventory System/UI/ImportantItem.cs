using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Infringed.InventorySystem.UI
{
    public class ImportantItem : MonoBehaviour
    {
        [field: SerializeField] public Image ItemImage { get; private set; }
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Graphic[] _additionalGraphics;
        private Graphic _graphicToMatch;

        private void Awake()
        {
            _graphicToMatch = transform.parent.GetComponentInParent<Graphic>();
        }

        public void InitializeItem(ItemData itemData)
        {
            _title.text = itemData.Name;
            _description.text = itemData.Description;
            ItemImage.sprite = itemData.BeltSprite;

            var status = _graphicToMatch.enabled;
            
            _title.enabled = status;
            _description.enabled = status;
            ItemImage.enabled = status;

            foreach (var g in _additionalGraphics)
            {
                g.enabled = status;
            }
        }
    }
}
