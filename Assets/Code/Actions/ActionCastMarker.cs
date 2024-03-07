using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.Actions
{
    public class ActionCastMarker : MonoBehaviour
    {
        public Transform Actor { get; set; }
        public Func<Vector3> GetTargetPosition;
    }
}
