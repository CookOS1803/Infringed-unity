using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Infringed.Math;

namespace Infringed.Map.Editor
{
    [CustomEditor(typeof(Floor))]
    public class FloorEditor : UnityEditor.Editor
    {
        private Floor _floor => target as Floor;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Update Walls"))
            {
                _floor.UpdateWalls();
                EditorUtility.SetDirty(_floor);
            }
        }

        private void OnSceneGUI()
        {
            foreach (var spline in _floor.Splines)
            {
                var nodeCount = spline.Nodes.Count;

                if (nodeCount == 0)
                    continue;

                DrawSplineNodes(spline);

                if (spline.Cap && nodeCount > 2)
                {
                    var from = _floor.transform.TransformPoint(spline.Nodes[nodeCount - 1]);
                    var to = _floor.transform.TransformPoint(spline.Nodes[0]);

                    Handles.DrawLine(from, to);
                }

                int interpolationCount;

                if (spline.InterpolatedPoints == null || (interpolationCount = spline.InterpolatedPoints.Length) < 2)
                    continue;

                Handles.color = Color.white;

                for (int i = 1; i < interpolationCount; i++)
                {
                    var from = _floor.transform.TransformPoint(spline.InterpolatedPoints[i - 1]);
                    var to = _floor.transform.TransformPoint(spline.InterpolatedPoints[i]);

                    Handles.DrawLine(from, to);
                }

                if (spline.Cap)
                {
                    var from = _floor.transform.TransformPoint(spline.InterpolatedPoints[interpolationCount - 1]);
                    var to = _floor.transform.TransformPoint(spline.InterpolatedPoints[0]);

                    Handles.DrawLine(from, to);
                }
            }

            _floor.UpdateSplineInterpolations();
        }

        private void DrawSplineNodes(Spline spline)
        {
            Handles.color = Color.red;

            DrawPositionHandle(spline, 0);

            for (int i = 1; i < spline.Nodes.Count; i++)
            {
                var from = _floor.transform.TransformPoint(spline.Nodes[i - 1]);
                var to = _floor.transform.TransformPoint(spline.Nodes[i]);

                Handles.DrawLine(from, to);

                DrawPositionHandle(spline, i);
            }
        }

        private void DrawPositionHandle(Spline spline, int index)
        {
            var globalNode0 = _floor.transform.TransformPoint(spline.Nodes[index]);
            var newGlobalNode0 = Handles.PositionHandle(globalNode0, Quaternion.identity);

            if (newGlobalNode0 != globalNode0)
            {
                EditorUtility.SetDirty(_floor);
            }

            spline.Nodes[index] = _floor.transform.InverseTransformPoint(newGlobalNode0);
        }

        private void testkal()
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
 