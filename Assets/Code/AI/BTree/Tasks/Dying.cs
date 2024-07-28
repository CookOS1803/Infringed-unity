using UnityEngine;
using Bonsai;
using Bonsai.Core.User;
using Infringed.Combat;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Tasks/Knight/")]
    public class Dying : FailableTask
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
            _enemy.OnEnemyDeathEnd += _OnDeath;
        }

        public override void OnExit()
        {

        }

        protected override Status _FailableRun()
        {
            return Status.Running;
        }

        private void _OnDeath(EnemyController sender)
        {
            _enemy.OnEnemyDeathEnd -= _OnDeath;
            _enemy.Dispose();
        }
        
    }
}
