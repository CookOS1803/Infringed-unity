using UnityEngine;
using Bonsai;
using Bonsai.Core;
using System.Collections.Generic;

namespace Bonsai.Core.User
{
    public abstract class ConditionalForFailables : InvertableConditional
    {
        [field: SerializeField] public bool IsForFailables { get; protected set; } = true; 

        /// <summary>
        /// Use _OnStart() instead
        /// </summary>
        public sealed override void OnStart()
        {
            if (!Blackboard.Contains(FailableTask.BLACKBOARD_NAME))
                Blackboard.Set(FailableTask.BLACKBOARD_NAME, new ConditionalList());
            
            _OnStart();
        }

        public override void OnEnter()
        {
            base.OnEnter();

            if (IsForFailables)
                Blackboard.Get<ConditionalList>(FailableTask.BLACKBOARD_NAME).Add(this);
        }

        public override void OnExit()
        {
            base.OnExit();

            if (IsForFailables)
                Blackboard.Get<ConditionalList>(FailableTask.BLACKBOARD_NAME).Remove(this);
        }

        protected abstract void _OnStart();
    }
}
