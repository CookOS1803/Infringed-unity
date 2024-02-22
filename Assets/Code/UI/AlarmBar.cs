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
        [Inject] private AIManager _aiManager;
        private TMP_Text _text;
        private Image _image;

        private void Start()
        {
            _text = GetComponent<TMP_Text>();
            _image = GetComponentInChildren<Image>();

            _DisableBar();

            //aiManager.onAlarmEnable += EnableBar;
            //aiManager.onAlarmDisable += DisableBar;
            //aiManager.onAlarmClockChange += UpdateBar;
        }

        private void _EnableBar()
        {
            _text.enabled = true;
            _image.enabled = true;
        }

        private void _DisableBar()
        {
            _text.enabled = false;
            _image.enabled = false;
        }

        private void _UpdateBar()
        {
            //image.fillAmount = aiManager.normalizedAlarmClock;
        }
    }
}