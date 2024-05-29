using UnityEngine;
using Bonsai;
using Bonsai.Core.User;
using System;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Tasks/Knight/")]
    public class GoToSuspect : FailableTask
    {
        private SuspicionController _suspicion;
        private VisionController _vision;
        private MovementController _movement;
        private SoundResponder _soundResponder;

        protected override void _OnStart()
        {
            _suspicion = Actor.GetComponent<SuspicionController>();
            _vision = Actor.GetComponent<VisionController>();
            _movement = Actor.GetComponent<MovementController>();
            _soundResponder = Actor.GetComponent<SoundResponder>();
        }

        public override void OnEnter()
        {
            _soundResponder.OnSound += _OnSound;
            _movement.SetDestination(_suspicion.SuspectPosition);
        }

        public override void OnExit()
        {
            _soundResponder.OnSound -= _OnSound;
        }

        protected override Status _FailableRun()
        {
            if (_movement.IsMoving)
            {
                _movement.SetDestination(_suspicion.SuspectPosition);

                return Status.Running;
            }

            return Status.Success;
        }

        private void _OnSound(Vector3 vector)
        {
            _suspicion.Suspect(vector);
        }
        
    }
}
