using System.Collections;
using System.Collections.Generic;
using Infringed.Player;
using UnityEngine;

namespace Infringed.Map
{
    public class Hideout : MonoBehaviour, IInteractable
    {
        public void Interact(PlayerController user)
        {
            user.Hide(this);
        }
    }
}
