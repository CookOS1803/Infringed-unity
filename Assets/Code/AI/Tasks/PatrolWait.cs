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
        private SuspicionController _suspicion;

        public override void OnStart()
        {
            _suspicion = Actor.GetComponent<SuspicionController>();
        }

        public override Status Run()
        {
            if (_suspicion.IsSuspecting)
                return Status.Failure;

            return base.Run();
        }
    }
}
