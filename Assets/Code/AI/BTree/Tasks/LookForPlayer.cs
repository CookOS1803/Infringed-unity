using UnityEngine;
using Bonsai;
using Bonsai.Core.User;
using UnityEngine.AI;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Tasks/Knight/")]
    public class LookForPlayer : FailableTask
    {
        [SerializeField] private float _lookingRadius = 6f;
        private EnemyController _enemy;
        private MovementController _movement;

        protected override void _OnStart()
        {
            _enemy = Actor.GetComponent<EnemyController>();
            _movement = Actor.GetComponent<MovementController>();
        }

        public override void OnEnter()
        {
            _movement.SetDestination(_GetRandomPosition());
        }

        protected override Status _FailableRun()
        {
            if (_movement.IsMoving)
                return Status.Running;

            return Status.Success;
        }
        
        private Vector3 _GetRandomPosition()
        {
            var randomDirection = UnityEngine.Random.insideUnitSphere * _lookingRadius;
            randomDirection += _enemy.LastKnownPlayerPosition;

            NavMesh.SamplePosition(randomDirection, out var hit, _lookingRadius * 2, 1);

            return hit.position;
        }
    }
}
