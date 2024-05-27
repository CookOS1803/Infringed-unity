using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Infringed.UI
{
    public class ToggleableGraphics : MonoBehaviour
    {
        public void SetGraphicsStatus(bool status)
        {
            var graphics = GetComponentsInChildren<Graphic>(includeInactive: true);
            
            foreach (var g in graphics)
            {
                g.enabled = status;
            }
        }

    }
}
