using UnityEngine;
using Bonsai;
using Bonsai.Core.User;

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
            _movement.SetDestination(_suspicion.SuspectPosition);
        }

        protected override Status _FailableRun()
        {
            if (_movement.IsMoving)
            {
                _suspicion.Suspect(_soundResponder.LastHeardSound);
                _movement.SetDestination(_suspicion.SuspectPosition);

                return Status.Running;
            }

            return Status.Success;
        }
        
    }
}
