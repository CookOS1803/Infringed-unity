using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Infringed.UI
{
    public class SwitchMenuButton : MonoBehaviour
    {
        [SerializeField] private GameObject _menu;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(_SwitchMenu);
        }

        private void _SwitchMenu()
        {
            transform.parent.gameObject.SetActive(false);
            _menu.SetActive(true);
        }
    }
}
