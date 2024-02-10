using System.Collections;
using System.Collections.Generic;
using System.Text;
using Bonsai;
using Bonsai.Core;
using UnityEngine;

namespace Infringed.AI
{
    [BonsaiNode("Tasks/Knight/")]
    public class RotatePatroler : Task
    {
        private Patroler _patroler;
        private VisionController _vision;

        public override void OnStart()
        {
            _patroler = Actor.GetComponent<Patroler>();
            _vision = Actor.GetComponent<VisionController>();
        }

        public override Status Run()
        {
            if (_vision.IsPlayerInView)
                return Status.Failure;

            var desiredRotation = _patroler.CurrentPatrolPoint.rotation;

            if (_vision.NoticeClock > _vision.SuspicionTime)
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
