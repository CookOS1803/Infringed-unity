using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bonsai.Designer.User
{
    [InitializeOnLoad]
    public class NodeColorSetter : INodeColorSetter
    {
        private static NodeColors _colors;

        static NodeColorSetter()
        {
            _colors = Resources.Load<NodeColors>("Node Colors");

            Drawer.colorSetter = new NodeColorSetter();
        }

        public Color NodeTypeColor(BonsaiNode node)
        {
            if (node.Behaviour is Core.Task)
            {
                if (node.Behaviour is Core.User.FailableTask)
                    return _colors.FailableTaskColor;

                return BonsaiPreferences.Instance.taskColor;
            }

            else if (node.Behaviour is Core.Service)
            {
                return BonsaiPreferences.Instance.serviceColor;
            }

            else if (node.Behaviour is Core.ConditionalAbort)
            {
                if (node.Behaviour is Core.User.ConditionalForFailables c && c.IsForFailables)
                    return _colors.ConditionalForFailablesColor;

                return BonsaiPreferences.Instance.conditionalColor;
            }

            else if (node.Behaviour is Core.Decorator)
            {
                return BonsaiPreferences.Instance.decoratorColor;
            }

            return BonsaiPreferences.Instance.compositeColor;
        }
    }
}
