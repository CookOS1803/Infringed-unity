using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hideout : MonoBehaviour, IInteractable
{
    public void Interact(PlayerController user)
    {
        user.Hide(this);
    }
}
