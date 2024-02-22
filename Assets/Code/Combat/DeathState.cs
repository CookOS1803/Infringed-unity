using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.Combat
{
    public class DeathState : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<IMortal>().OnDeath();
        }
    }
}
