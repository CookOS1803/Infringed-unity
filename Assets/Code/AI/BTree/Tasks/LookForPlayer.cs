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
            Vector3 randomDirection;
            NavMeshHit hit = default;

            var radius = _lookingRadius;

            do
            {
                randomDirection = UnityEngine.Random.insideUnitSphere * radius;
                randomDirection += _enemy.LastKnownPlayerPosition;

                radius -= 0.5f;

            } while (radius > 0f && !NavMesh.SamplePosition(randomDirection, out hit, _lookingRadius, 1));

            return hit.position;
        }
    }
}
