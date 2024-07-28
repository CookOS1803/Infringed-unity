using UnityEngine;
using Bonsai;
using Bonsai.Core.User;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Tasks/Knight/")]
    public class WaitWhileSuspecting : FailableTask
    {
        [SerializeField] private float _time = 3f;
        private SuspicionController _suspicion;
        private VisionController _vision;
        private SoundResponder _soundResponder;
        private MovementController _movement;
        private bool _heardSound = false;

        protected override void _OnStart()
        {
            Blackboard.Set("Suspicion Clock", 0f);
            _suspicion = Actor.GetComponent<SuspicionController>();
            _vision = Actor.GetComponent<VisionController>();
            _soundResponder = Actor.GetComponent<SoundResponder>();
            _movement = Actor.GetComponent<MovementController>();
        }

        public override void OnEnter()
        {
            _soundResponder.OnSound += _OnSound;
        }

        public override void OnExit()
        {
            _soundResponder.OnSound -= _OnSound;

            Blackboard.Set("Suspicion Clock", 0f);
            _heardSound = false;
        }

        protected override Status _FailableRun()
        {
            if (_heardSound)
            {
                _movement.SetDestination(_soundResponder.LastHeardSound);
                return Status.Failure;
            }

            var clock = Blackboard.Get<float>("Suspicion Clock");
            if (clock < _time)
            {
                Blackboard.Set("Suspicion Clock", clock += Time.fixedDeltaTime);

                return Status.Running;
            }

            _suspicion.IsSuspecting = false;

            return Status.Success;
        }  

        private void _OnSound(Vector3 vector)
        {
            _heardSound = true;
        }      
    }
}
