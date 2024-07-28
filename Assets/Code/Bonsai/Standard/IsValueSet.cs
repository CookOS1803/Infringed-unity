using UnityEngine;
using System;
using System.Text;
using Bonsai.Core.User;
using Bonsai.Core;

namespace Bonsai.Standard.User
{
    [BonsaiNode("Conditional/User/", "Condition")]
    public class IsValueSet : ConditionalForFailables
    {
        [Tooltip("The key to check if it has a value set.")]
        public string key;

        private Action<Blackboard.KeyEvent> OnBlackboardChanged;

        protected override void _OnStart()
        {
            OnBlackboardChanged = delegate (Blackboard.KeyEvent e)
            {
                if (key == e.Key)
                {
                    Evaluate();
                }
            };
        }

        protected override bool _InvertableCondition()
        {
            return Blackboard.IsSet(key);
        }

        protected override void OnObserverBegin()
        {
            Blackboard.AddObserver(OnBlackboardChanged);
        }

        protected override void OnObserverEnd()
        {
            Blackboard.RemoveObserver(OnBlackboardChanged);
        }

        public override void Description(StringBuilder builder)
        {
            base.Description(builder);
            builder.AppendLine();

            if (key == null || key.Length == 0)
            {
                builder.Append("No key is set to check");
            }
            else
            {
                builder.AppendFormat("Blackboard key: {0}", key);
            }
        }

    }
}
