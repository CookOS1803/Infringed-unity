using System;
using System.Collections;
using System.Collections.Generic;
using Infringed.Quests;
using UnityEngine;
using Zenject;

namespace Infringed.Player
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float _smoothing = 1f;
        [SerializeField] private float _dialogSmoothing = 1f;
        [SerializeField] private float _mouseFactor = 1f;
        [SerializeField] private float _additionalMouseFactor = 10f;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private Vector3 _dialogOffset;
        [SerializeField] private Vector3 _winPosition = new Vector3(-13.5f, 6.28f, 2.36f);
        [SerializeField] private Vector3 _winEulerAngles = new Vector3(49.4f, 45f, 0f);
        [Inject] private PlayerController _player;
        private float _currentMouseFactor;
        private DialogueGiver _giver;

        private void Awake()
        {
            _player.OnInitiateDialogue += _OnInitiateDialogue;
        }

        private void OnDestroy()
        {
            _player.OnInitiateDialogue -= _OnInitiateDialogue;
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
            if (_giver != null)
                _DialogPositioning();
            else
                _BasePositioning();
        }

        private void _BasePositioning()
        {
            var mousePosition = Input.mousePosition;
            mousePosition.x /= Screen.width;
            mousePosition.y /= Screen.height;
            mousePosition.z = mousePosition.y;
            mousePosition.y = 0f;

            mousePosition.x -= 0.5f;
            mousePosition.z -= 0.5f;

            var targetCamPos = _player.transform.position + _offset + mousePosition * _currentMouseFactor;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, _smoothing * Time.deltaTime);
            transform.forward = Vector3.Lerp(transform.forward, -_offset.normalized, _smoothing * Time.deltaTime);

        }

        private void _DialogPositioning()
        {
            transform.position = Vector3.Lerp(transform.position, _giver.transform.position + _dialogOffset, _dialogSmoothing * Time.deltaTime);
            transform.forward = Vector3.Lerp(transform.forward, -_dialogOffset.normalized, _dialogSmoothing * Time.deltaTime);
        }

        public void OnWin()
        {
            transform.position = _winPosition;
            transform.eulerAngles = _winEulerAngles;

            Destroy(this);
        }

        private void _OnInitiateDialogue(DialogueGiver giver)
        {
            giver.OnDialogueEnd += _OnDialogueEnd;
            _giver = giver;
        }

        private void _OnDialogueEnd(DialogueGiver giver)
        {
            giver.OnDialogueEnd -= _OnDialogueEnd;
            _giver = null;
        }
    }
}
