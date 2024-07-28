using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Infringed.InventorySystem.UI
{
    public class ItemDescription : MonoBehaviour
    {
        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponentInChildren<TMP_Text>(includeInactive: true);
        }

        public void UpdateInfo(string name, string description)
        {
            _text.text = name;

            if (description != string.Empty)
                _text.text += "\n\n" + description;
        }
    }
}
