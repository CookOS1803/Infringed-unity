using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonsai.Designer.User
{
    public class BonsaiEditor : Bonsai.Designer.BonsaiEditor
    {
        public override void PollInput(Event e, CanvasTransform t, Rect inputRect)
        {
            base.PollInput(e, t, inputRect);

            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete)
            {
                RemoveSelectedNodes();
                UpdateAbortableSelection();
            }
        }
    }
}
