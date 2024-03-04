using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using Infringed.AI;

namespace Infringed.UI
{
    public class AlarmBar : MonoBehaviour
    {
        [SerializeField] private AIManager _aiManager;
        private TMP_Text _text;
        private Image _image;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _image = GetComponentInChildren<Image>();
        }

        private void Update()
        {
            _text.enabled = _aiManager.Alarm;
            _image.enabled = _aiManager.Alarm;

            _image.fillAmount = _aiManager.AlarmClock / _aiManager.AlarmTime;
        }
    }
}