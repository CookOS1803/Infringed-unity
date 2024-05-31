using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed
{
    public class FpsCap : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _targetFramerate = 20;
        private bool _active = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Application.targetFrameRate = _active ? -1 : _targetFramerate;
                _active = !_active;
            }
        }
    }
}
