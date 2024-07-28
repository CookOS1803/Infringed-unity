using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Infringed.UI
{
    public class LogEntryUI : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _lifetime = 5f;
        [SerializeField, Min(0f)] private float _fadeoutStart = 3f;
        private TMP_Text _text;
        private float _clock = 0f;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void Update()
        {
            if (_clock >= _lifetime)
            {
                Destroy(gameObject);

                return;
            }

            if (_clock >= _fadeoutStart)
            {
                var fadeoutTime = _lifetime - _fadeoutStart;
                var fadeoutClock = _clock - _fadeoutStart;

                var color = _text.color;
                color.a = 1f - fadeoutClock / fadeoutTime;
                _text.color = color;
            }

            _clock += Time.deltaTime;
        }

        public void Initialize(string message, Color color)
        {
            _text.text = message;
            _text.color = color;
        }
    }
}
