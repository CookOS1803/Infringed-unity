using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Infringed.Map.Editor
{
    [CustomEditor(typeof(Floor))]
    public class FloorEditor : UnityEditor.Editor
    {
        private Floor _floor => (Floor)target;

        private void OnSceneGUI()
        {
            var cameraRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            var floorWasHit = Physics.Raycast(cameraRay, out var floorHit, Mathf.Infinity);

            if (!floorWasHit || floorHit.transform != _floor.transform)
                return;

            var position = floorHit.point;
            var size = 2f;
            var pickSize = size * 2f;

            var direction = -Camera.current.transform.forward;
            var rotation = Quaternion.LookRotation(direction);

            if (Handles.Button(position, rotation, size, pickSize, Handles.RectangleHandleCap))
                Debug.Log("The button was pressed!");

            if (Event.current.button == 1)
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("dada"), false, () => Debug.Log("The button was pressed!"));

                menu.ShowAsContext();
            }

        }
    }
}
 