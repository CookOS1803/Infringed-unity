using System.Collections;
using System.Collections.Generic;
using System.Text;
using Bonsai;
using Bonsai.Core;
using UnityEngine;

namespace Infringed.AI
{
    [BonsaiNode("Tasks/Knight/")]
    public class PatrolTask : Task
    {
        [SerializeField] private bool _changePoint = false;
        private Patroler _patroler;

        public override void OnStart()
        {
            _patroler = Actor.GetComponent<Patroler>();
        }

        public override void OnEnter()
        {
            _patroler.GoToNextPoint();
        }

        public override Status Run()
        {
            if (_patroler.IsOnTheWay)
                return Status.Running;            

            if (_changePoint)
                _patroler.ChangePoint();

            return Status.Success;
        }
    
        public override void Description(StringBuilder builder)
        {
            builder.Append("Go to next patrol point");

            if (_changePoint)
                builder.Append(", then change it");
        }
    }
}
