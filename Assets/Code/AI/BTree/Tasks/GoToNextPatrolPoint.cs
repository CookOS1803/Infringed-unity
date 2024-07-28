using System.Collections;
using System.Collections.Generic;
using System.Text;
using Bonsai;
using Bonsai.Core.User;
using UnityEngine;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Tasks/Knight/")]
    public class GoToNextPatrolPoint : FailableTask
    {
        private Patroler _patroler;

        protected override void _OnStart()
        {
            _patroler = Actor.GetComponent<Patroler>();
        }

        public override void OnEnter()
        {
            _patroler.GoToNextPoint();
        }

        protected override Status _FailableRun()
        {
            if (_patroler.IsOnTheWay)
                return Status.Running;
            
            return Status.Success;
        }
    }
}
