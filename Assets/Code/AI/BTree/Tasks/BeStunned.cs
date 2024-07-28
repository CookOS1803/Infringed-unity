using UnityEngine;
using Bonsai;
using Bonsai.Core.User;
using Infringed.Combat;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Tasks/Knight/")]
    public class BeStunned : FailableTask
    {
        [SerializeField, Min(0f)] private float _stunSourceDirectionMultiplier = 1f;
        private MovementController _movement;
        private StunController _stunController;
        private SuspicionController _suspicion;
        private VisionController _vision;
        private EnemyController _enemy;

        protected override void _OnStart()
        {
            _movement = Actor.GetComponent<MovementController>();
            _stunController = Actor.GetComponent<StunController>();
            _suspicion = Actor.GetComponent<SuspicionController>();
            _vision = Actor.GetComponent<VisionController>();
            _enemy = Actor.GetComponent<EnemyController>();
        }

        public override void OnEnter()
        {
            _movement.CanMove = false;
            _vision.enabled = false;
            _suspicion.enabled = false;
        }

        public override void OnExit()
        {
            _movement.CanMove = true;
            _vision.enabled = true;
            _suspicion.enabled = true;

            if (!_enemy.IsDying)
            {
                var position = Actor.transform.position;
                Vector3 playerPosition;

                if (Physics.Raycast(position + Vector3.up, _stunController.StunSourceDirection, out var hit, _stunSourceDirectionMultiplier))
                {
                    playerPosition = hit.point - Vector3.up;
                }
                else
                    playerPosition = position + _stunController.StunSourceDirection * _stunSourceDirectionMultiplier;
                    
                _enemy.Alarm(playerPosition);
            }
        }

        protected override Status _FailableRun()
        {
            if (_stunController.IsStunned)
                return Status.Running;

            return Status.Success;
        }
        
    }
}
