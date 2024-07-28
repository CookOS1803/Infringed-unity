using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Infringed.UI
{
    public class SwitchMenuButton : MonoBehaviour
    {
        [SerializeField] private GameObject _currentMenu;
        [SerializeField] private GameObject _nextMenu;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(_SwitchMenu);
        }

        private void _SwitchMenu()
        {
            _currentMenu.SetActive(false);
            _nextMenu.SetActive(true);
        }
    }
}
