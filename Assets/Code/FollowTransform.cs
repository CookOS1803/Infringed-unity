using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed
{
    public class FollowTransform : MonoBehaviour
    {
        [SerializeField] private Transform _followed;
        
        private void Update()
        {
            transform.position = _followed.position;
        }
    }
}
