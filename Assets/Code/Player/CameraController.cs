using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Infringed.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float _smoothing = 1f;
        [SerializeField] private float _mouseFactor = 1f;
        [SerializeField] private float _additionalMouseFactor = 10f;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private Vector3 _winPosition = new Vector3(-13.5f, 6.28f, 2.36f);
        [SerializeField] private Vector3 _winEulerAngles = new Vector3(49.4f, 45f, 0f);
        private Transform _target;
        private float _currentMouseFactor;

        [Inject]
        private void _SetTarget(PlayerController player)
        {
            _target = player.transform;
        }

        private void Start()
        {
            _currentMouseFactor = _mouseFactor;
        }

        private void Update()
        {
            _currentMouseFactor = Input.GetButton("View") ? _additionalMouseFactor : _mouseFactor;
        }

        private void LateUpdate()
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.x /= Screen.width;
            mousePosition.y /= Screen.height;
            mousePosition.z = mousePosition.y;
            mousePosition.y = 0f;

            mousePosition.x -= 0.5f;
            mousePosition.z -= 0.5f;

            Vector3 targetCamPos = _target.position + _offset + mousePosition * _currentMouseFactor;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, _smoothing * Time.deltaTime);
        }

        public void OnWin()
        {
            transform.position = _winPosition;
            transform.eulerAngles = _winEulerAngles;

            Destroy(this);
        }
    }
}
