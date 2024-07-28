using UnityEngine;
using Bonsai;
using Bonsai.Core.User;
using Infringed.Combat;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Conditional/Knight/")]
    public class IsStunned : ConditionalForFailables
    {
        private StunController _stunController;

        protected override void _OnStart()
        {
            _stunController = Actor.GetComponent<StunController>();
        }

        protected override bool _InvertableCondition()
        {
            return _stunController.IsStunned;
        }

    }
}
