using UnityEngine;
using Bonsai;
using Bonsai.Core;

namespace Infringed.AI
{
    [BonsaiNode("Tasks/Knight/")]
    public class WaitWhileSuspecting : Task
    {
        private VisionController _vision;

        public override void OnStart()
        {
            _vision = Actor.GetComponent<VisionController>();
        }

        public override Status Run()
        {
            if (_vision.NoticeClock > 0)
                return Status.Success;
            
            Blackboard.Set("Is Suspecting", false);

            return Status.Failure;
        }
        
    }
}
