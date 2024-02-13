using UnityEngine;
using Bonsai;
using Bonsai.Core;
using Bonsai.Standard;

namespace Infringed.AI
{
    [BonsaiNode("Tasks/Knight/")]
    public class WaitWhileSuspecting : Task
    {
        [SerializeField] private float _time = 3f;
        private SuspicionController _suspicion;
        private VisionController _vision;

        public override void OnStart()
        {
            Blackboard.Set("Suspicion Clock", 0f);
            _suspicion = Actor.GetComponent<SuspicionController>();
            _vision = Actor.GetComponent<VisionController>();
        }

        public override Status Run()
        {
            if (_vision.IsPlayerInView)
                return Status.Failure;


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
        
    }
}
