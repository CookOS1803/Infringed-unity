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
            var difference = _wallsParent.childCount - Splines.Count;

            if (difference > 0)
            {
                while (difference > 0)
                {
                    DestroyImmediate(_wallsParent.GetChild(0).gameObject);
                    difference--;
                }
            }
            else if (difference < 0)
            {
                while (difference < 0)
                {
                    Instantiate(_wallsPrefab, _wallsParent);
                    difference++;
                }
            }

            foreach (var spline in Splines)
            {
                
            }
        }
#endif


    }
}
