using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Infringed.UI
{
    public class ToggleableContainer : MonoBehaviour
    {
        [field: SerializeField] public ToggleableGraphics Toggleable { get; private set; }
        public Toggle Toggle { get; private set; }

        private void Awake()
        {
            Toggle = GetComponent<Toggle>();
        }

        public void MatchToggleAndToggleable()
        {
            Toggleable.SetGraphicsStatus(Toggle.isOn);
        }
    }
}
