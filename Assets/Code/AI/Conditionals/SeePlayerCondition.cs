using System.Collections;
using System.Collections.Generic;
using System.Text;
using Bonsai;
using Bonsai.Core;
using UnityEngine;

namespace Infringed.AI
{
    [BonsaiNode("Conditional/Knight/")]
    public class SeePlayerCondition : ConditionalAbort
    {
        [SerializeField] private bool _invert = false;
        private VisionController _vision;

        public override void OnStart()
        {
            base.OnStart();
            
            _vision = Actor.GetComponent<VisionController>();
        }
    
        public override void Description(StringBuilder builder)
        {
            base.Description(builder);
        }

        public override bool Condition()
        {
            return _vision.IsPlayerInView ^ _invert;
        }
    }
}
