using UnityEngine;
using Bonsai;
using Bonsai.Core;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Tasks/Knight/")]
    public class GoToSuspect : Task
    {
        private SuspicionController _suspicion;
        private VisionController _vision;
        private MovementController _movement;

        public override void OnStart()
        {
            _suspicion = Actor.GetComponent<SuspicionController>();
            _vision = Actor.GetComponent<VisionController>();
            _movement = Actor.GetComponent<MovementController>();
        }

        public override void OnEnter()
        {
            _movement.SetDestination(_suspicion.SuspectPosition);
        }

        public override Status Run()
        {
            if (_vision.IsPlayerInView)
                return Status.Failure;

            if (_movement.IsMoving)
                return Status.Running;

            return Status.Success;
        }
        
    }
}
