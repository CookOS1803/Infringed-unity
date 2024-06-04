using System;
using System.Collections;
using System.Collections.Generic;
using Infringed.Player;
using TMPro;
using UnityEngine;

namespace Infringed.InventorySystem.UI
{
    public class TestEssentia : MonoBehaviour
    {
        public event Action OnChange;

        [SerializeField] private TMP_Text _text;
        private PlayerController _player;
        public int Essentia { get; private set; } = 0;

        private void Awake()
        {
            _player = FindObjectOfType<PlayerController>();
            _player.OnEssentia += OnEssentia;
        }

        private void Update()
        {
            _text.text = "Эссенция: " + Essentia;
        }

        private void OnDestroy()
        {
            _player.OnEssentia -= OnEssentia;
        }

        private void OnEssentia()
        {
            Essentia++;
            OnChange?.Invoke();
        }

        public bool Consume(int cost)
        {
            var delta = Essentia - cost;

            if (delta < 0)
                return false;

            Essentia = delta;
            return true;
        }
    }
}
