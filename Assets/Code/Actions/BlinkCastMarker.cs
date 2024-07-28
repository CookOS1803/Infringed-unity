using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Infringed.Actions
{
    public class BlinkCastMarker : ActionCastMarker
    {
        private BlinkAction _action => Action as BlinkAction;
        
        private void Update()
        {
            transform.position = BlinkAction.GetBlinkPosition(Actor, GetTargetPosition(), _action.MaxBlinkDistance);
        }
    }
}
