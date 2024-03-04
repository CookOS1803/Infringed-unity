using UnityEngine;
using Bonsai;
using Bonsai.Core.User;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Tasks/Knight/")]
    public class AttackPlayer : FailableTask
    {
        private MovementController _movement;
        private EnemyController _enemy;

        protected override void _OnStart()
        {
            _movement = Actor.GetComponent<MovementController>();
            _enemy = Actor.GetComponent<EnemyController>();
        }

        public override void OnEnter()
        {
            _movement.CanMove = false;
            _enemy.transform.LookAt(_enemy.LastKnownPlayerPosition);
            _enemy.Attack();
        }

        public override void OnExit()
        {
            _movement.CanMove = true;
        }

        protected override Status _FailableRun()
        {
            if (_enemy.IsAttacking)
                return Status.Running;

            return Status.Success;
        }
        
    }
}
