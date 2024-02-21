using System.Collections;
using System.Collections.Generic;
using System.Text;
using Bonsai;
using Bonsai.Core;
using UnityEngine;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Tasks/Knight/")]
    public class GoToNextPatrolPoint : Task
    {
        private Patroler _patroler;
        private SuspicionController _suspicion;

        public override void OnStart()
        {
            _patroler = Actor.GetComponent<Patroler>();
            _suspicion = Actor.GetComponent<SuspicionController>();
        }

        public override void OnEnter()
        {
            if (_suspicion.IsSuspecting || Blackboard.IsSet("Sound Source"))
                return;

            _patroler.GoToNextPoint();
        }

        public override Status Run()
        {
            if (_suspicion.IsSuspecting || Blackboard.IsSet("Sound Source"))
                return Status.Failure;

            if (_patroler.IsOnTheWay)
                return Status.Running;
            
            return Status.Success;
        }
    }
}
