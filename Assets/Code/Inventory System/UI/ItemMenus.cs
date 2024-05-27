using System;
using System.Collections;
using System.Collections.Generic;
using Infringed.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Infringed.InventorySystem.UI
{
    public class ItemMenus : MonoBehaviour
    {
        [SerializeField] private Transform _generalGraphicsParent;
        [SerializeField] private ToggleGroup _menuToggles;
        private PlayerInput _input;
        private bool _graphicsStatus = true;
        private ToggleableContainer[] _toggles;

        private void Awake()
        {
            _input = GetComponent<PlayerInput>();
            _toggles = _menuToggles.GetComponentsInChildren<ToggleableContainer>();

            _input.actions["OpenClose"].performed += _SwitchGraphicsStatus;
        }

        private void Start()
        {
            _SwitchGraphicsStatus(default);
        }

        private void OnDestroy()
        {
            _input.actions["OpenClose"].performed -= _SwitchGraphicsStatus;
        }

        private void _SwitchGraphicsStatus(InputAction.CallbackContext context)
        {
            var graphics = _generalGraphicsParent.GetComponentsInChildren<Graphic>(includeInactive: true);

            _graphicsStatus = !_graphicsStatus;

            foreach (var g in graphics)
            {
                g.enabled = _graphicsStatus;
            }

            Action<ToggleableContainer> action;
            action = _graphicsStatus ? t => t.MatchToggleAndToggleable()
                                     : t => t.Toggleable.SetGraphicsStatus(false);

            foreach (var toggle in _toggles)
            {
                action(toggle);
            }
        }
    }
}
