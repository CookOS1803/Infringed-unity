using UnityEngine;
using Bonsai;
using Bonsai.Core;

namespace Infringed.AI
{
    [BonsaiNode("Tasks/Knight/")]
    public class LookAtPlayer : Task
    {
        private VisionController _vision;
        private MovementController _movement;

        public override void OnStart()
        {
            _vision = Actor.GetComponent<VisionController>();
            _movement = Actor.GetComponent<MovementController>();
        }

        public override Status Run()
        {
            if (_vision.IsPlayerInView)
            {
                Actor.transform.LookAt(_vision.LastNoticedPlayer.transform);
                _movement.CanMove = false;
                Blackboard.Set("Suspicion Clock", 0f);

                return Status.Running;
            }

            _movement.CanMove = true;

            return Status.Failure;
        }

    }
}
