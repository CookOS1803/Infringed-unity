using System.Collections;
using System.Collections.Generic;
using System.Text;
using Bonsai;
using Bonsai.Core;
using Bonsai.Standard;
using UnityEngine;

namespace Infringed.AI
{
    [BonsaiNode("Tasks/Knight/")]
    public class PatrolWait : Wait
    {
        private VisionController _visionController;

        public override void OnStart()
        {
            _visionController = Actor.GetComponent<VisionController>();
        }

        public override Status Run()
        {
            if (_visionController.IsPlayerInView)
                return Status.Failure;

            return base.Run();
        }
    }
}
