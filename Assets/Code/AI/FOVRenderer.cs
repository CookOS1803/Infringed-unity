using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.AI
{
    public class FOVRenderer : MonoBehaviour
    {
        [SerializeField, Min(2)] private int _rayCount = 2;
        private Mesh _mesh;
        private VisionController _vision;
        private float _angleInc => _vision.FieldOfView / _rayCount;
        private float _scaleFactor => 1f / transform.parent.localScale.z;

        private void Start()
        {
            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;

            _vision = GetComponentInParent<VisionController>();
        }

        private void Update()
        {
            var vertices = new Vector3[_rayCount + 2];
            var triangles = new int[_rayCount * 3];
            var angle = -_vision.FieldOfView / 2 - transform.eulerAngles.y;

            vertices[0] = Vector3.zero;
            vertices[1] = _ApplyObstacle(_GetVertex(angle));

            angle += _angleInc;

            var vertexIndex = 2;
            var triangleIndex = 0;

            for (int i = 1; i <= _rayCount; i++)
            {
                vertices[vertexIndex] = _ApplyObstacle(_GetVertex(angle));

                triangles[triangleIndex++] = 0;
                triangles[triangleIndex++] = vertexIndex - 1;
                triangles[triangleIndex++] = vertexIndex;

                vertexIndex++;
                angle += _angleInc;
            }

            _mesh.vertices = vertices;
            _mesh.triangles = triangles;

            _mesh.RecalculateBounds();
        }

        private Vector3 _GetVertex(float angle)
        {
            return Quaternion.Euler(0f, angle, 0f) * transform.forward * _vision.DistanceOfView * _scaleFactor;
        }

        private Vector3 _ApplyObstacle(Vector3 vertex)
        {
            if (Physics.Linecast(transform.position, transform.position + transform.parent.TransformDirection(vertex / _scaleFactor), out var hit))
            {
                vertex *= Vector3.Distance(transform.position, hit.point) / _vision.DistanceOfView;
            }

            return vertex;
        }
    }
}