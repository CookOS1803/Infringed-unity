using System.Collections;
using System.Collections.Generic;
using Infringed.Math;
using UnityEngine;

namespace Infringed.Map
{
    public class Floor : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private GameObject _wallsPrefab;
        [SerializeField] private Transform _wallsParent;
        [SerializeField, Min(0f)] private float _wallWidth = 2f;
        [SerializeField, Min(0f)] private float _wallHeight = 2f;
        [field: SerializeField] public List<Spline> Splines { get; private set; } = new();

        public void UpdateSplineInterpolations()
        {
            foreach (var spline in Splines)
            {
                spline.UpdateInterpolation();
            }
        }

        public void UpdateWalls()
        {
            _UpdateWallsObjects();

            for (int i = 0; i < Splines.Count; i++)
            {
                _BuildWallMesh(i);
            }
        }

        private void _UpdateWallsObjects()
        {
            var difference = _wallsParent.childCount - Splines.Count;

            if (difference > 0)
            {
                while (difference > 0)
                {
                    DestroyImmediate(_wallsParent.GetChild(0).gameObject);
                    difference--;
                }
            }
            else
            {
                while (difference < 0)
                {
                    Instantiate(_wallsPrefab, _wallsParent);
                    difference++;
                }
            }
        }

        private void _BuildWallMesh(int splineIndex)
        {
            var spline = Splines[splineIndex];
            var meshFilter = _wallsParent.GetChild(splineIndex).GetComponent<MeshFilter>();
            var mesh = new Mesh();
            meshFilter.mesh = mesh;

            var length = spline.InterpolatedPoints.Length;
            var vertexCount = length * 4;
            var triangleCount = ((length - 1) * 8 + 4) * 3;

            if (spline.Cap)
                triangleCount += 4 * 3;

            var vertices = new Vector3[vertexCount];
            var triangles = new int[triangleCount];

            if (spline.Cap)
            {
                var current = spline.InterpolatedPoints[0];
                var prevDirection = (current - spline.InterpolatedPoints[length - 1]).normalized;
                var currDirection = (spline.InterpolatedPoints[1] - current).normalized;
                _MakeVertices(vertices, 0, current, prevDirection, currDirection);                

                current = spline.InterpolatedPoints[length - 1];
                prevDirection = (current - spline.InterpolatedPoints[length - 2]).normalized;
                currDirection = (spline.InterpolatedPoints[0] - current).normalized;
                _MakeVertices(vertices, length - 1, current, prevDirection, currDirection);

                _MakeTrianglesForLast(triangles);
            }
            else
            {
                _CapWallEnds(vertices, triangles, spline);
            }

            _MakeTriangles(triangles, 0);

            for (int i = 1; i < length - 1; i++)
            {
                var current = spline.InterpolatedPoints[i];
                var prevDirection = (current - spline.InterpolatedPoints[i - 1]).normalized;
                var currDirection = (spline.InterpolatedPoints[i + 1] - current).normalized;
                _MakeVertices(vertices, i, current, prevDirection, currDirection);

                _MakeTriangles(triangles, i);
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }

        private void _CapWallEnds(Vector3[] vertices, int[] triangles, Spline spline)
        {
            var halfWidth = _wallWidth / 2f;
            var length = spline.InterpolatedPoints.Length;
            var vertexCount = vertices.Length;
            var triangleCount = triangles.Length;

            var first = spline.InterpolatedPoints[0];
            var second = spline.InterpolatedPoints[1];
            var direction = (second - first).normalized;
            var normal = Quaternion.AngleAxis(90f, direction) * Vector3.up;

            vertices[0] = first + normal * halfWidth;
            vertices[1] = first - normal * halfWidth;
            vertices[2] = first - normal * halfWidth + Vector3.up * _wallHeight;
            vertices[3] = first + normal * halfWidth + Vector3.up * _wallHeight;

            var beforeLast = spline.InterpolatedPoints[length - 2];
            var last = spline.InterpolatedPoints[length - 1];
            direction = (last - beforeLast).normalized;
            normal = Quaternion.AngleAxis(90f, direction) * Vector3.up;

            vertices[vertexCount - 4] = last + normal * halfWidth;
            vertices[vertexCount - 3] = last - normal * halfWidth;
            vertices[vertexCount - 2] = last - normal * halfWidth + Vector3.up * _wallHeight;
            vertices[vertexCount - 1] = last + normal * halfWidth + Vector3.up * _wallHeight;        

            triangles[triangleCount - 12] = 0;
            triangles[triangleCount - 11] = 2;
            triangles[triangleCount - 10] = 1;
            triangles[triangleCount - 9] = 0;
            triangles[triangleCount - 8] = 3;
            triangles[triangleCount - 7] = 2;

            triangles[triangleCount - 6] = vertexCount - 4;
            triangles[triangleCount - 5] = vertexCount - 3;
            triangles[triangleCount - 4] = vertexCount - 2;
            triangles[triangleCount - 3] = vertexCount - 4;
            triangles[triangleCount - 2] = vertexCount - 2;
            triangles[triangleCount - 1] = vertexCount - 1;
        }

        private void _MakeVertices(Vector3[] vertices, int i, Vector3 current, Vector3 prevDirection, Vector3 currDirection)
        {
            var halfWidth = _wallWidth / 2f;

            var angle = (Vector3.Angle(prevDirection, currDirection) - 180f) / 2f;
            var angleRad = angle * Mathf.Deg2Rad;
            var bisector = Vector3.RotateTowards(currDirection, prevDirection, angleRad, 0f);

            if (Vector3.Cross(prevDirection, currDirection).y < 0f)
                bisector = Quaternion.Euler(0f, 180f, 0f) * bisector;
            var width = halfWidth / Mathf.Sin(angleRad);

            vertices[i * 4] = current + bisector * width;
            vertices[i * 4 + 1] = current - bisector * width;
            vertices[i * 4 + 2] = current - bisector * width + Vector3.up * _wallHeight;
            vertices[i * 4 + 3] = current + bisector * width + Vector3.up * _wallHeight;
        }

        private static void _MakeTriangles(int[] triangles, int i)
        {
            // bottom
            triangles[i * 8 * 3] = i * 4;
            triangles[i * 8 * 3 + 1] = i * 4 + 1;
            triangles[i * 8 * 3 + 2] = i * 4 + 5;
            triangles[i * 8 * 3 + 3] = i * 4;
            triangles[i * 8 * 3 + 4] = i * 4 + 5;
            triangles[i * 8 * 3 + 5] = i * 4 + 4;

            // top
            triangles[i * 8 * 3 + 6] = i * 4 + 3;
            triangles[i * 8 * 3 + 7] = i * 4 + 7;
            triangles[i * 8 * 3 + 8] = i * 4 + 6;
            triangles[i * 8 * 3 + 9] = i * 4 + 3;
            triangles[i * 8 * 3 + 10] = i * 4 + 6;
            triangles[i * 8 * 3 + 11] = i * 4 + 2;

            // left
            triangles[i * 8 * 3 + 12] = i * 4;
            triangles[i * 8 * 3 + 13] = i * 4 + 4;
            triangles[i * 8 * 3 + 14] = i * 4 + 7;
            triangles[i * 8 * 3 + 15] = i * 4;
            triangles[i * 8 * 3 + 16] = i * 4 + 7;
            triangles[i * 8 * 3 + 17] = i * 4 + 3;

            // right
            triangles[i * 8 * 3 + 18] = i * 4 + 1;
            triangles[i * 8 * 3 + 19] = i * 4 + 2;
            triangles[i * 8 * 3 + 20] = i * 4 + 6;
            triangles[i * 8 * 3 + 21] = i * 4 + 1;
            triangles[i * 8 * 3 + 22] = i * 4 + 6;
            triangles[i * 8 * 3 + 23] = i * 4 + 5;
        }

        private static void _MakeTrianglesForLast(int[] triangles)
        {
            var length = triangles.Length;
            var vertexCount = (length / 24) * 4;
            var last = vertexCount - 4;

            // bottom
            triangles[length - 24] = last;
            triangles[length - 23] = last + 1;
            triangles[length - 22] = 1;
            triangles[length - 21] = last;
            triangles[length - 20] = 1;
            triangles[length - 19] = 0;

            // top
            triangles[length - 18] = last + 3;
            triangles[length - 17] = 3;
            triangles[length - 16] = 2;
            triangles[length - 15] = last + 3;
            triangles[length - 14] = 2;
            triangles[length - 13] = last + 2;

            // left
            triangles[length - 12] = last;
            triangles[length - 11] = 0;
            triangles[length - 10] = 3;
            triangles[length - 9] = last;
            triangles[length - 8] = 3;
            triangles[length - 7] = last + 3;

            // right
            triangles[length - 6] = last + 1;
            triangles[length - 5] = last + 2;
            triangles[length - 4] = 2;
            triangles[length - 3] = last + 1;
            triangles[length - 2] = 2;
            triangles[length - 1] = 1;
        }
#endif


    }
}
