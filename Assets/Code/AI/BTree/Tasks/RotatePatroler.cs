using System.Collections;
using System.Collections.Generic;
using System.Text;
using Bonsai;
using Bonsai.Core;
using UnityEngine;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Tasks/Knight/")]
    public class RotatePatroler : Task
    {
        private Patroler _patroler;
        private SuspicionController _suspicion;

        public override void OnStart()
        {
            _patroler = Actor.GetComponent<Patroler>();
            _suspicion = Actor.GetComponent<SuspicionController>();
        }

        public override Status Run()
        {
            if (_suspicion.IsSuspecting || Blackboard.IsSet("Sound Source"))
                return Status.Failure;

            var desiredRotation = _patroler.CurrentPatrolPoint.rotation;

            if (_patroler.transform.rotation != desiredRotation)
            {
                _patroler.transform.rotation = Quaternion.RotateTowards(_patroler.transform.rotation, desiredRotation, _patroler.AngularSpeed * Time.deltaTime);

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
