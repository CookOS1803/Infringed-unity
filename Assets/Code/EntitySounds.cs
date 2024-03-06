using System;
using System.Collections;
using System.Collections.Generic;
using Infringed.Combat;
using UnityEngine;
using Zenject;

namespace Infringed
{
    public class EntitySounds : MonoBehaviour
    {
        [Inject] protected CustomAudio _customAudio;
        protected AudioController _audio;
        protected Health _health;

        protected virtual void Awake()
        {
            _audio = GetComponent<AudioController>();
            _health = GetComponent<Health>();
        }

        protected virtual void OnEnable()
        {
            _health.OnDamageTaken += _OnDamageTaken;
        }

        protected virtual void OnDisable()
        {
            _health.OnDamageTaken -= _OnDamageTaken;
        }

        protected virtual void _OnDamageTaken()
        {
            _audio.Play(_customAudio.WeaponHit);
        }

        public virtual void OnStepEvent(AnimationEvent animationEvent)
        {
            _audio.Play(_customAudio.GetRandomStep());
        }

        public virtual void OnSwingEvent()
        {
            _audio.Play(_customAudio.WeaponSwing);
        }
    }
}
