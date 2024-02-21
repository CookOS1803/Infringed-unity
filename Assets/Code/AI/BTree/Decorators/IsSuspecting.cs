using UnityEngine;
using Bonsai;
using Bonsai.Core.User;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Conditional/Knight/")]
    public class IsSuspecting : InvertableConditional
    {
        private SuspicionController _suspicion;

        public override void OnStart()
        {
            _suspicion = Actor.GetComponent<SuspicionController>();
        }

        protected override bool _InvertableCondition()
        {
            return _suspicion.IsSuspecting;
        }
    }
}
