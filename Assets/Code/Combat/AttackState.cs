using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.Combat
{
    public class AttackState : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<IAttacker>().AttackStarted();
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<IAttacker>().AttackEnded();
        }
    }
}
