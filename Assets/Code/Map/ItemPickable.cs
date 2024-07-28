using System.Collections;
using System.Collections.Generic;
using Infringed.InventorySystem;
using Infringed.Player;
using UnityEngine;

namespace Infringed.Map
{
    public class ItemPickable : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemData _data;

        public ItemData GetItem()
        {
            return _data;
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }

        public void Interact(PlayerController user)
        {
            user.PickItem(this);
        }
    }
}
