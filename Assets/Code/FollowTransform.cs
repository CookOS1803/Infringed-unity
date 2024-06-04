using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed
{
    public class FollowTransform : MonoBehaviour
    {
        [SerializeField] private Transform _followed;
        [SerializeField] private Vector3 _offset;
        
        private void Update()
        {
            transform.position = _followed.position + _offset;
        }
    }
}
