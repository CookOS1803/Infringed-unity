using System;
using System.Collections;
using System.Collections.Generic;
using Infringed.Player;
using TMPro;
using UnityEngine;

namespace Infringed.UI
{
    public class TestEssentia : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        private PlayerController _player;
        public int _essentia = 0;

        private void Awake()
        {
            _player = FindObjectOfType<PlayerController>();
            _player.OnEssentia += OnEssentia;
        }

        private void Update()
        {
            _text.text = "Эссенция: " + _essentia;
        }

        private void OnDestroy()
        {
            _player.OnEssentia -= OnEssentia;
        }

        private void OnEssentia()
        {
            _essentia++;
        }

        public bool Consume()
        {
            if (_essentia == 0)
                return false;
            
            _essentia--;
            return true;
        }
    }
}
