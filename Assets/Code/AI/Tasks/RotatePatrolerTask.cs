using System.Collections;
using System.Collections.Generic;
using System.Text;
using Bonsai;
using Bonsai.Core;
using UnityEngine;

namespace Infringed.AI
{
    [BonsaiNode("Tasks/Knight/")]
    public class RotatePatrolerTask : Task
    {
        [SerializeField] private bool _changePoint = true;
        private Patroler _patroler;

        public override void OnStart()
        {
            _patroler = Actor.GetComponent<Patroler>();
        }

        public override Status Run()
        {
            var desiredRotation = _patroler.CurrentPatrolPoint.rotation;

            if (_patroler.transform.rotation != desiredRotation)
            {
                _patroler.transform.rotation = Quaternion.RotateTowards(_patroler.transform.rotation, desiredRotation, _patroler.AngularSpeed * Time.deltaTime);

                return Status.Running;
            }

            if (_changePoint)
                _patroler.ChangePoint();

            return Status.Success;
        }

        public override void Description(StringBuilder builder)
        {
            builder.Append("Rotate along patrol point");

            if (_changePoint)
                builder.Append(", then change it");
        }
    }
}
