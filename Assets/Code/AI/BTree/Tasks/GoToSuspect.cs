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

        protected override void _OnStart()
        {
            _suspicion = Actor.GetComponent<SuspicionController>();
            _vision = Actor.GetComponent<VisionController>();
            _movement = Actor.GetComponent<MovementController>();
        }

        public override void OnEnter()
        {
            _movement.SetDestination(_suspicion.SuspectPosition);
        }

        protected override Status _FailableRun()
        {
            if (_movement.IsMoving)
                return Status.Running;

            return Status.Success;
        }
        
    }
}
