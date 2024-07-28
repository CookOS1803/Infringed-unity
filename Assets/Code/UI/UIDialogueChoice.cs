using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Infringed.UI
{
    public class UIDialogueChoice : MonoBehaviour
    {
        public TMP_Text Text { get; set; }
        public Button Button { get; set; }

        private void Awake()
        {
            Text = GetComponent<TMP_Text>();
            Button = GetComponent<Button>();
        }
    }
}
