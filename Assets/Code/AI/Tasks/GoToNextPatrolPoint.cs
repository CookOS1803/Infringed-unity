using System.Collections;
using System.Collections.Generic;
using System.Text;
using Bonsai;
using Bonsai.Core;
using UnityEngine;

namespace Infringed.AI
{
    [BonsaiNode("Tasks/Knight/")]
    public class GoToNextPatrolPoint : Task
    {
        private Patroler _patroler;
        private VisionController _visionController;

        public override void OnStart()
        {
            _patroler = Actor.GetComponent<Patroler>();
            _visionController = Actor.GetComponent<VisionController>();
        }

        public override void OnEnter()
        {
            _patroler.GoToNextPoint();
        }

        public override Status Run()
        {
            if (_visionController.IsPlayerInView)
                return Status.Failure;

            if (_patroler.IsOnTheWay)
                return Status.Running;
            
            return Status.Success;
        }
    }
}
