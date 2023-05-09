using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlayerDeathActivator : MonoBehaviour
{
    [Zenject.Inject] private PlayerController player;
    private bool currentStatus = false;

    private void Awake()
    {
        SetStatus(false);

        player.onDeath += () => SetStatus(true); 
    }

    private void SetStatus(bool status)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(status);
        }
    }

    public void SwitchStatus()
    {
        currentStatus = !currentStatus;
        SetStatus(currentStatus);
    }
}
