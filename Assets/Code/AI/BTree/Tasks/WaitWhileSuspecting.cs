using UnityEngine;
using Bonsai;
using Bonsai.Core;
using Bonsai.Standard;
using System;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Tasks/Knight/")]
    public class WaitWhileSuspecting : Task
    {
        [SerializeField] private float _time = 3f;
        private SuspicionController _suspicion;
        private VisionController _vision;
        private SoundResponder _soundResponder;
        private bool _failNextRun;

        public override void OnStart()
        {
            Blackboard.Set("Suspicion Clock", 0f);
            _suspicion = Actor.GetComponent<SuspicionController>();
            _vision = Actor.GetComponent<VisionController>();
            _soundResponder = Actor.GetComponent<SoundResponder>();
        }

        public override void OnEnter()
        {
            //_soundResponder.OnSound += _OnSound;
        }

        public override void OnExit()
        {
            //_soundResponder.OnSound -= _OnSound;            
        }

        public override Status Run()
        {
            if (_vision.IsPlayerInView || _failNextRun)
            {
                _failNextRun = false;
                
                return Status.Failure;
            }

            var clock = Blackboard.Get<float>("Suspicion Clock");
            if (clock < _time)
            {
                Blackboard.Set("Suspicion Clock", clock += Time.deltaTime);

                return Status.Running;
            }

            Blackboard.Set("Suspicion Clock", 0f);
            _suspicion.IsSuspecting = false;

            return Status.Success;
        }

        private void _OnSound(Vector3 vector)
        {
            _failNextRun = true;
        }
        
    }
}
