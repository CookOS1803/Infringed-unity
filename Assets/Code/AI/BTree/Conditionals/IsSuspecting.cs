using UnityEngine;
using Bonsai;
using Bonsai.Core.User;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Conditional/Knight/")]
    public class IsSuspecting : ConditionalForFailables
    {
        private SuspicionController _suspicion;

        protected override void _OnStart()
        {
            _suspicion = Actor.GetComponent<SuspicionController>();
        }

        protected override bool _InvertableCondition()
        {
            return _suspicion.IsSuspecting;
        }

    }
}
