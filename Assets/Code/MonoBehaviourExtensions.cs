using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed
{
    public static class MonoBehaviourExtensions
    {
        public static void DeferForNextFrame(this MonoBehaviour self, Action action)
        {
            self.StartCoroutine(_CoroutineNextFrame(action));
        }

        private static IEnumerator _CoroutineNextFrame(Action action)
        {
            yield return new WaitForEndOfFrame();

            action();
        }
    }
}
