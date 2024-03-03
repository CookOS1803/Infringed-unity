using UnityEngine;
using Bonsai;
using Bonsai.Core.User;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Tasks/Knight/")]
    public class ChasePlayer : FailableTask
    {
        private MovementController _movement;
        private EnemyController _enemy;

        protected override void _OnStart()
        {
            _movement = Actor.GetComponent<MovementController>();
            _enemy = Actor.GetComponent<EnemyController>();
        }

        protected override Status _FailableRun()
        {
            _movement.SetDestination(_enemy.LastKnownPlayerPosition);

            if (_movement.IsMoving)
            {
                return Status.Running;
            }

            return Status.Success;
        }
    }
}
