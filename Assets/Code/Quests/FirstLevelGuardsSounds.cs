using System;
using System.Collections;
using System.Collections.Generic;
using Infringed.Combat;
using UnityEngine;
using Zenject;

namespace Infringed.Quests
{
    public class FirstLevelGuardsSounds : MonoBehaviour
    {
        [Inject] private CustomAudio _customAudio;
        private AudioController _audio;
        private Health _health;

        private void Awake()
        {
            _audio = GetComponent<AudioController>();
        }

        private void OnDestroy()
        {
            if (_health)
                Deinitialize();
        }

        public void Initialize(Health health)
        {
            _health = health;
            _health.OnDamageTaken += _OnDamageTaken;
        }

        public void Deinitialize()
        {
            _health.OnDamageTaken -= _OnDamageTaken;
        }

        private void _OnDamageTaken()
        {
            Deinitialize();
            _audio.Play(_customAudio.WeaponHit);
        }
    }
}
