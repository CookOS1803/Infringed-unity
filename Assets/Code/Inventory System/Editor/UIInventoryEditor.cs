using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Infringed.InventorySystem.UI.Editor
{
    [CustomEditor(typeof(UIInventory))]
    public class UIInventoryEditor : UnityEditor.Editor
    {
        private UIInventory _uiInventory => (UIInventory)target;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            
        }
    }
}
