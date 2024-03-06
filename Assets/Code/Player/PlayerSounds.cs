using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.Player
{
    public class PlayerSounds : EntitySounds
    {
        [SerializeField] private AudioClip _hideoutEnter;
        [SerializeField] private AudioClip _hideoutExit;
        private PlayerController _player;

        protected override void Awake()
        {
            base.Awake();

            _player = GetComponent<PlayerController>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _player.OnHide += _OnHide;
            _player.OnExitHideout += _OnExitHideout;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _player.OnHide -= _OnHide;
            _player.OnExitHideout -= _OnExitHideout;
        }

        public override void OnStepEvent(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
                base.OnStepEvent(animationEvent);
        }

        private void _OnHide()
        {
            _audio.Play(_hideoutEnter);
        }

        private void _OnExitHideout()
        {
            _audio.Play(_hideoutExit);
        }
    }
}
