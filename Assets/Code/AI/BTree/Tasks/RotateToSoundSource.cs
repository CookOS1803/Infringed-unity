using UnityEngine;
using Bonsai;
using Bonsai.Core.User;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Tasks/Knight/")]
    public class RotateToSoundSource : FailableTask
    {
        [SerializeField] private float _hearTime = 2f;
        private MovementController _movement;
        private SuspicionController _suspicion;
        private VisionController _vision;
        private SoundResponder _soundResponder;
        private float _hearClock;

        protected override void _OnStart()
        {
            _movement = Actor.GetComponent<MovementController>();
            _suspicion = Actor.GetComponent<SuspicionController>();
            _vision = Actor.GetComponent<VisionController>();
            _soundResponder = Actor.GetComponent<SoundResponder>();
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

        protected override Status _FailableRun()
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
            var position = Actor.transform.position;
            var soundSource = Blackboard.Get<Vector3>("Sound Source");

            var actorToSource = (soundSource - position).normalized;
            actorToSource.y = 0f;
            var actorToActualSource = (_soundResponder.LastHeardSound - Actor.transform.position).normalized;
            actorToActualSource.y = 0f;

            var angle = Vector3.Angle(actorToActualSource, actorToSource);

            if (angle < 45)
            {
                actorToSource = actorToActualSource;
                Blackboard.Set("Sound Source", actorToActualSource);
            }

            var desiredRotation = Quaternion.FromToRotation(Vector3.forward, actorToSource);
            Actor.transform.rotation = Quaternion.RotateTowards(Actor.transform.rotation, desiredRotation, _movement.AngularSpeed * Time.deltaTime);
        }

    }
}
