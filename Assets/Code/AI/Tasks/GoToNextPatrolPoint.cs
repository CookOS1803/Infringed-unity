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
        private VisionController _vision;

        public override void OnStart()
        {
            _patroler = Actor.GetComponent<Patroler>();
            _vision = Actor.GetComponent<VisionController>();
        }

        public override void OnEnter()
        {
            _patroler.GoToNextPoint();
        }

        public override Status Run()
        {
            if (_vision.NoticeClock > _vision.SuspicionTime)
                return Status.Failure;

            if (_patroler.IsOnTheWay)
                return Status.Running;
            
            return Status.Success;
        }
    }
}
