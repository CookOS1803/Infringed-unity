using System.Collections;
using System.Collections.Generic;
using Infringed.Combat;
using UnityEngine;
using Zenject;

namespace Infringed.AI
{
    public class EnemySounds : MonoBehaviour
    {
        [Inject] private CustomAudio _customAudio;
        private AudioController _audio;
        private Health _health;

        private void Awake()
        {
            _audio = GetComponent<AudioController>();
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.OnDamageTaken += _OnDamageTaken;
        }

        private void OnDisable()
        {
            _health.OnDamageTaken -= _OnDamageTaken;
        }

        private void _OnDamageTaken()
        {
            _audio.Play(_customAudio.WeaponHit);
        }

        public void OnStepEvent()
        {
            _audio.Play(_customAudio.GetRandomStep());
        }
    }
}
