using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.UI
{
    public class Billboard : MonoBehaviour
    {
        private void Update()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}
