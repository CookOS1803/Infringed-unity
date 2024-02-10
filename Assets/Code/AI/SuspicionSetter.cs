using UnityEngine;
using Bonsai;
using Bonsai.Core;

namespace Infringed.AI
{
    [BonsaiNode("Decorators/Knight/")]
    public class SuspicionSetter : Decorator
    {
        public override void OnStart()
        {
            Blackboard.Set("Is Suspecting", false);
        }

        public override void OnEnter()
        {
            if (!Blackboard.Get<bool>("Is Suspecting"))
                base.OnEnter();
        }

        public override Status Run()
        {
            var childStatus = Iterator.LastChildExitStatus.GetValueOrDefault(Status.Failure);

            if (childStatus == Status.Failure)
            {
                Blackboard.Set("Is Suspecting", true);
            }

            return childStatus;
        }
        
    }
}
