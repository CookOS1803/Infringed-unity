using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.Player
{
    public class PlayerSounds : EntitySounds
    {
        private PlayerController _player;

        protected override void Awake()
        {
            base.Awake();

            _player = GetComponent<PlayerController>();
        }

        public override void OnStepEvent(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
                base.OnStepEvent(animationEvent);
        }
    }
}
