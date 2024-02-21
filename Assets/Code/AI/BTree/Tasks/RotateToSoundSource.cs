using UnityEngine;
using Bonsai;
using Bonsai.Core;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Tasks/Knight/")]
    public class RotateToSoundSource : Task
    {
        [SerializeField] private float _hearTime = 2f;
        private MovementController _movement;
        private SuspicionController _suspicion;
        private VisionController _vision;
        private float _hearClock;

        public override void OnStart()
        {
            _movement = Actor.GetComponent<MovementController>();
            _suspicion = Actor.GetComponent<SuspicionController>();
            _vision = Actor.GetComponent<VisionController>();
        }

        public override void OnEnter()
        {
            if (Blackboard.IsSet("Sound Source"))
            {
                _hearClock = 0f;
                _suspicion.Suspect(Blackboard.Get<Vector3>("Sound Source"));
                _movement.CanMove = false;
            }
        }

        public override void OnExit()
        {
            _movement.CanMove = true;
            Blackboard.Unset("Sound Source");
        }

        public override Status Run()
        {
            if (_vision.IsPlayerInView || Blackboard.IsUnset("Sound Source"))
                return Status.Failure;

            if (_hearClock < _hearTime)
            {
                _Rotate();

                _hearClock += Time.deltaTime;

                return Status.Running;
            }

            return Status.Success;
        }

        private void _Rotate()
        {
            var soundSource = Blackboard.Get<Vector3>("Sound Source");
            var actorToSource = soundSource - Actor.transform.position;
            actorToSource.y = 0f;
            var desiredRotation = Quaternion.FromToRotation(Vector3.forward, actorToSource.normalized);
            Actor.transform.rotation = Quaternion.RotateTowards(Actor.transform.rotation, desiredRotation, _movement.AngularSpeed * Time.deltaTime);
        }

    }
}
