using UnityEngine;
using Bonsai;
using Bonsai.Core.User;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Conditional/Knight/")]
    public class IsPlayerInView : ConditionalForFailables
    {
        private VisionController _vision;

        protected override void _OnStart()
        {
            _vision = Actor.GetComponent<VisionController>();
        }

        protected override bool _InvertableCondition()
        {
            return _vision.IsPlayerInView;
        }

    }
}
