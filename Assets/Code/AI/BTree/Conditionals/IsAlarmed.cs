using UnityEngine;
using Bonsai;
using Bonsai.Core.User;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Conditional/Knight/")]
    public class IsAlarmed : ConditionalForFailables
    {
        private EnemyController _enemy;

        protected override void _OnStart()
        {
            _enemy = Actor.GetComponent<EnemyController>();
        }

        protected override bool _InvertableCondition()
        {
            return _enemy.IsAlarmed;
        }
    }
}
