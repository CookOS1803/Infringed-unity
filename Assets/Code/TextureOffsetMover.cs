using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed
{
    public class TextureOffsetMover : MonoBehaviour
    {
        [SerializeField] private Vector2 _offsetDirection = Vector2.right;
        private Renderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }

        private void Update()
        {
            _renderer.material.SetTextureOffset("_MainTex", _offsetDirection * Time.time);
        }
    }
}
