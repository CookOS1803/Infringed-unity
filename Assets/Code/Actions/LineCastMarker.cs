using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Infringed.Actions
{
    public class LineCastMarker : ActionCastMarker
    {
        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            var direction = (GetTargetPosition() - Actor.position);
            direction.y = 0f;
            direction.Normalize();
            Physics.Raycast(Actor.position + Vector3.up, direction, out var hit, 100f, ~LayerMask.GetMask("Floor"));

            var target = hit.point;
            target.y = Actor.position.y;

            _lineRenderer.SetPosition(0, Actor.position);
            _lineRenderer.SetPosition(1, target);
        }
    }
}
