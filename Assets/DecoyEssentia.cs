using System.Collections;
using System.Collections.Generic;
using Infringed.Map;
using Infringed.Player;
using UnityEngine;

namespace Infringed
{
    public class DecoyEssentia : MonoBehaviour, IInteractable
    {
        public void Interact(PlayerController user)
        {
            user.ConsumeEssentia();

            Destroy(gameObject);
        }
    }
}
