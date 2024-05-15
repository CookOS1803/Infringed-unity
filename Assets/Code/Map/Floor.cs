using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infringed.Math;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Infringed.Map
{
    [ExecuteInEditMode]
    public class Floor : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private GameObject _wallsPrefab;
        [SerializeField] private Transform _wallsParent;
        [SerializeField, Min(0f)] private float _wallWidth = 2f;
        [SerializeField, Min(0f)] private float _wallHeight = 2f;
        [field: SerializeField] public List<Spline> Splines { get; private set; } = new();

        private void LateUpdate()
        {
            if (Selection.activeGameObject == gameObject)
                UpdateWalls();
        }

        public void UpdateSplineInterpolations()
        {
            foreach (var spline in Splines)
            {
                spline.UpdateInterpolation();
            }
        }

        public void UpdateWalls()
        {
            if (this == null)
                return;

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
                    var instance = Instantiate(_wallsPrefab, _wallsParent);
                    difference++;
                }
            }
        }

        private void _BuildWallMesh(int splineIndex)
        {
            var spline = Splines[splineIndex];
            var meshFilter = _wallsParent.GetChild(splineIndex).GetComponent<MeshFilter>();

            var length = spline.InterpolatedPoints.Length;
            var vertexCount = length * 16;
            var triangleCount = ((length - 1) * 8 + 4) * 3;

            if (spline.Cap)
                triangleCount += 4 * 3;
            
            var vertices = new Vector3[vertexCount];
            var triangles = new int[triangleCount];

            _MakeTriangles(triangles, 0);

            for (int i = 1; i < length - 1; i++)
            {
                var current = spline.InterpolatedPoints[i];
                var prevDirection = (current - spline.InterpolatedPoints[i - 1]).normalized;
                var currDirection = (spline.InterpolatedPoints[i + 1] - current).normalized;
                _MakeVertices(vertices, i, current, prevDirection, currDirection);

                _MakeTriangles(triangles, i);
            }

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

                vertexCount -= 8;

                vertices = vertices.Take(vertexCount).ToArray();
            }

            var mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            meshFilter.mesh = mesh;
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

            // start vertices for floor, ceiling and walls
            vertices[0] = first + normal * halfWidth;
            vertices[1] = first + normal * halfWidth;
            vertices[4] = first - normal * halfWidth;
            vertices[5] = first - normal * halfWidth;
            vertices[8] = first - normal * halfWidth + Vector3.up * _wallHeight;
            vertices[9] = first - normal * halfWidth + Vector3.up * _wallHeight;
            vertices[12] = first + normal * halfWidth + Vector3.up * _wallHeight;
            vertices[13] = first + normal * halfWidth + Vector3.up * _wallHeight;

            // start cap vertices
            vertices[2] = first + normal * halfWidth;
            vertices[3] = first - normal * halfWidth;
            vertices[6] = first - normal * halfWidth + Vector3.up * _wallHeight;
            vertices[7] = first + normal * halfWidth + Vector3.up * _wallHeight;

            var beforeLast = spline.InterpolatedPoints[length - 2];
            var last = spline.InterpolatedPoints[length - 1];
            direction = (last - beforeLast).normalized;
            normal = Quaternion.AngleAxis(90f, direction) * Vector3.up;

            // end vertices for floor, ceiling and walls
            vertices[vertexCount - 16] = last + normal * halfWidth;
            vertices[vertexCount - 15] = last + normal * halfWidth;
            vertices[vertexCount - 14] = last - normal * halfWidth;
            vertices[vertexCount - 13] = last - normal * halfWidth;
            vertices[vertexCount - 12] = last - normal * halfWidth + Vector3.up * _wallHeight;
            vertices[vertexCount - 11] = last - normal * halfWidth + Vector3.up * _wallHeight;
            vertices[vertexCount - 10] = last + normal * halfWidth + Vector3.up * _wallHeight;
            vertices[vertexCount - 9] = last + normal * halfWidth + Vector3.up * _wallHeight;

            // end cap vertices
            vertices[10] = last + normal * halfWidth;
            vertices[11] = last - normal * halfWidth;
            vertices[14] = last - normal * halfWidth + Vector3.up * _wallHeight;
            vertices[15] = last + normal * halfWidth + Vector3.up * _wallHeight;

            // start cap triangles
            triangles[triangleCount - 12] = 2;
            triangles[triangleCount - 11] = 6;
            triangles[triangleCount - 10] = 3;
            triangles[triangleCount - 9] = 2;
            triangles[triangleCount - 8] = 7;
            triangles[triangleCount - 7] = 6;

            // end cap triangles
            triangles[triangleCount - 6] = 10;
            triangles[triangleCount - 5] = 14;
            triangles[triangleCount - 4] = 15;
            triangles[triangleCount - 3] = 10;
            triangles[triangleCount - 2] = 11;
            triangles[triangleCount - 1] = 14;

            var ti = (spline.InterpolatedPoints.Length - 2) * 24;

            // fix triangle indices before end
            // bottom
            triangles[ti + 2] = vertexCount - 14;
            triangles[ti + 4] = vertexCount - 14;
            triangles[ti + 5] = vertexCount - 16;
            // top
            triangles[ti + 7] = vertexCount - 10;
            triangles[ti + 8] = vertexCount - 12;
            triangles[ti + 10] = vertexCount - 12;
            // left
            triangles[ti + 13] = vertexCount - 15;
            triangles[ti + 14] = vertexCount - 9;
            triangles[ti + 16] = vertexCount - 9;
            // right
            triangles[ti + 20] = vertexCount - 11;
            triangles[ti + 22] = vertexCount - 11;
            triangles[ti + 23] = vertexCount - 13;


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

            setVertices(0);
            setVertices(1);
            setVertices(2);
            setVertices(3);

            void setVertices(int j)
            {
                vertices[i * 16 + j] = current + bisector * width;
                vertices[i * 16 + 4 + j] = current - bisector * width;
                vertices[i * 16 + 8 + j] = current - bisector * width + Vector3.up * _wallHeight;
                vertices[i * 16 + 12 + j] = current + bisector * width + Vector3.up * _wallHeight;
            }
        }

        private static void _MakeTriangles(int[] triangles, int i)
        {
            const int vd = 4;
            const int vn = vd * 4 + 2;

            var ti = i * 8 * 3;
            var vi = i * 16;

            // bottom
            triangles[ti] = vi;
            triangles[ti + 1] = vi + vd;
            triangles[ti + 2] = vi + vd + vn;
            triangles[ti + 3] = vi;
            triangles[ti + 4] = vi + vd + vn;
            triangles[ti + 5] = vi + vn;

            // top
            triangles[ti + 6] = vi + 3 * vd;
            triangles[ti + 7] = vi + 3 * vd + vn;
            triangles[ti + 8] = vi + 2 * vd + vn;
            triangles[ti + 9] = vi + 3 * vd;
            triangles[ti + 10] = vi + 2 * vd + vn;
            triangles[ti + 11] = vi + 2 * vd;

            // left
            triangles[ti + 12] = vi + 1;
            triangles[ti + 13] = vi + 1 + vn;
            triangles[ti + 14] = vi + 1 + 3 * vd + vn;
            triangles[ti + 15] = vi + 1;
            triangles[ti + 16] = vi + 1 + 3 * vd + vn;
            triangles[ti + 17] = vi + 1 + 3 * vd;

            // right
            triangles[ti + 18] = vi + 1 + vd;
            triangles[ti + 19] = vi + 1 + 2 * vd;
            triangles[ti + 20] = vi + 1 + 2 * vd + vn;
            triangles[ti + 21] = vi + 1 + vd;
            triangles[ti + 22] = vi + 1 + 2 * vd + vn;
            triangles[ti + 23] = vi + 1 + vd + vn;
        }

        private static void _MakeTrianglesForLast(int[] triangles)
        {
            const int vd = 4;

            var length = triangles.Length;
            var vertexCount = (length / 24) * 16;
            var last = vertexCount - 16;

            // bottom
            triangles[length - 24] = last;
            triangles[length - 23] = last + vd;
            triangles[length - 22] = vd + 2;
            triangles[length - 21] = last;
            triangles[length - 20] = vd + 2;
            triangles[length - 19] = 2;

            // top
            triangles[length - 18] = last + 3 * vd;
            triangles[length - 17] = 3 * vd + 2;
            triangles[length - 16] = 2 * vd + 2;
            triangles[length - 15] = last + 3 * vd;
            triangles[length - 14] = 2 * vd + 2;
            triangles[length - 13] = last + 2 * vd;

            // left
            triangles[length - 12] = last + 1;
            triangles[length - 11] = 3;
            triangles[length - 10] = 3 + 3 * vd;
            triangles[length - 9] = last + 1;
            triangles[length - 8] = 3 + 3 * vd;
            triangles[length - 7] = last + 1 + 3 * vd;

            // right
            triangles[length - 6] = last + 1 + vd;
            triangles[length - 5] = last + 1 + 2 * vd;
            triangles[length - 4] = 3 + 2 * vd;
            triangles[length - 3] = last + 1 + vd;
            triangles[length - 2] = 3 + 2 * vd;
            triangles[length - 1] = 3 + vd;
        }
#endif


    }
}
