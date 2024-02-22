using UnityEngine;
using Infringed.Player;

namespace Infringed.Map
{
    public interface IInteractable
    {
        void Interact(PlayerController user);
    }
}
