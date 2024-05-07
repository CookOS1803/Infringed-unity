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

        protected override void _OnStart()
        {
            Blackboard.Set("Suspicion Clock", 0f);
            _suspicion = Actor.GetComponent<SuspicionController>();
            _vision = Actor.GetComponent<VisionController>();
        }

        protected override Status _FailableRun()
        {
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
