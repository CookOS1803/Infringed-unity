using UnityEngine;
using Bonsai;
using Bonsai.Core.User;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Tasks/Knight/")]
    public class LookAtPlayer : FailableTask
    {
        private VisionController _vision;
        private MovementController _movement;
        private SuspicionController _suspicion;
        private EnemyController _enemy;

        protected override void _OnStart()
        {
            _vision = Actor.GetComponent<VisionController>();
            _movement = Actor.GetComponent<MovementController>();
            _suspicion = Actor.GetComponent<SuspicionController>();
            _enemy = Actor.GetComponent<EnemyController>();
        }

        public override void OnEnter()
        {
            _movement.CanMove = false;
        }

        public override void OnExit()
        {
            Blackboard.Set("Suspicion Clock", 0f);
            _movement.CanMove = true;
        }

        protected override Status _FailableRun()
        {
            if (_suspicion.NoticeClock == _suspicion.NoticeTime)
            {
                _enemy.Alarm();
                
                return Status.Failure;
            }

            if (_vision.IsPlayerInView)
            {
                Actor.transform.LookAt(_vision.LastNoticedPlayer.transform);

                return Status.Running;
            }

            return Status.Success;
        }

    }
}
