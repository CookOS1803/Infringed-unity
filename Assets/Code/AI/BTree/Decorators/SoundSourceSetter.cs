using UnityEngine;
using Bonsai;
using Bonsai.Core;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Decorators/Knight/")]
    public class SoundSourceSetter : Decorator
    {
        private SoundResponder _soundResponder;

        public override void OnStart()
        {
            _soundResponder = Actor.GetComponent<SoundResponder>();
        }

        public override void OnEnter()
        {
            _soundResponder.OnSound += _SetSoundSource;

            base.OnEnter();
        }

        public override void OnExit()
        {
            _soundResponder.OnSound -= _SetSoundSource;
        }

        public override Status Run()
        {
            return Iterator.LastChildExitStatus.GetValueOrDefault(Status.Failure);
        }

        private void _SetSoundSource(Vector3 source)
        {
            Blackboard.Set("Sound Source", source);
        }
        
    }
}
