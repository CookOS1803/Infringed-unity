using UnityEngine;
using Bonsai;
using Bonsai.Core;
using System.Collections.Generic;

namespace Bonsai.Core.User
{
    public abstract class FailableTask : Task
    {
        public const string BLACKBOARD_NAME = "Conditionals for failables";

        /// <summary>
        /// Use _OnStart() instead
        /// </summary>
        public sealed override void OnStart()
        {
            if (!Blackboard.Contains(BLACKBOARD_NAME))
                Blackboard.Set(BLACKBOARD_NAME, new ConditionalList());
            
            _OnStart();
        }

        /// <summary>
        /// Use _FailableRun() instead
        /// </summary>
        public sealed override Status Run()
        {
            foreach (var condition in Blackboard.Get<ConditionalList>(BLACKBOARD_NAME))
            {
                if (!condition.Condition())
                    return Status.Failure;
            }

            return _FailableRun();
        }

        public override BehaviourNode[] GetReferencedNodes()
        {
            var node = Parent;
            var refs = new List<BehaviourNode>();

            while (node != null)
            {
                if (node is ConditionalForFailables n && n.IsForFailables)
                    refs.Add(node);

                node = node.Parent;
            }

            return refs.ToArray();
        }

        protected virtual void _OnStart() {}
        
        protected abstract Status _FailableRun();
    }
}
