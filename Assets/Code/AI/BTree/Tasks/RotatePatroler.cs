using System.Collections;
using System.Collections.Generic;
using System.Text;
using Bonsai;
using Bonsai.Core.User;
using UnityEngine;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Tasks/Knight/")]
    public class RotatePatroler : FailableTask
    {
        private Patroler _patroler;

        protected override void _OnStart()
        {
            _patroler = Actor.GetComponent<Patroler>();
        }

        protected override Status _FailableRun()
        {
            var desiredRotation = _patroler.CurrentPatrolPoint.rotation;

            if (_patroler.transform.rotation != desiredRotation)
            {
                _patroler.transform.rotation = Quaternion.RotateTowards(_patroler.transform.rotation, desiredRotation, _patroler.AngularSpeed * Time.fixedDeltaTime);

                return Status.Running;
            }

            _patroler.ChangePoint();

            return Status.Success;
        }

        public override void Description(StringBuilder builder)
        {
            builder.Append("Rotate along patrol point");
        }
    }
}
