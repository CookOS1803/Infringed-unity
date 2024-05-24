using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace Infringed.UI.Editor
{
    [CustomEditor(typeof(NonDrawingGraphic))]
    public class NonDrawingGraphicEditor : GraphicEditor
    {
        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
		    EditorGUILayout.PropertyField(base.m_Script);
		    // skipping AppearanceControlsGUI
		    base.RaycastControlsGUI();
		    base.serializedObject.ApplyModifiedProperties();
        }
    }
}
