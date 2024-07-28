using System.Collections;
using System.Collections.Generic;
using Infringed.Player;
using UnityEngine;
using Zenject;

namespace Infringed.Map
{
    public class WinTrigger : MonoBehaviour
    {
        [SerializeField] private OnPlayerDeathActivator _menu;
        [Inject] private CameraController _camera;

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                Destroy(player.gameObject);
                _camera.OnWin();
                _menu.SetStatus(true);
            }
        }
    }
}
