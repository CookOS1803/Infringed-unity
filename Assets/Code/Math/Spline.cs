using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Infringed.Math
{
    [System.Serializable]
    public class Spline
    {
        [field: SerializeField] public bool Cap { get; set; }
        [field: SerializeField] public List<Vector3> Nodes { get; set; } = new();
        [field: SerializeField, Min(1)] public int InterpolationModifier { get; set; } = 5;
        public Vector3[] InterpolatedPoints { get; set; }

        public Spline()
        {
            UpdateInterpolation();
        }

        public void UpdateInterpolation()
        {
            if (InterpolationModifier == 1)
                InterpolatedPoints = Nodes.ToArray();
            else
                InterpolatedPoints = CubicInterpolation.InterpolateXZ(Nodes, Nodes.Count * InterpolationModifier);
        }
    }
}
