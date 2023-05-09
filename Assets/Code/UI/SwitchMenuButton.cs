using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchMenuButton : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(SwitchMenu);
    }

    private void SwitchMenu()
    {
        transform.parent.gameObject.SetActive(false);
        menu.SetActive(true);
    }
}
