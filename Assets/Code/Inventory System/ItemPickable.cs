using System.Collections;
using System.Collections.Generic;
using Infringed.Map;
using Infringed.Player;
using UnityEngine;

namespace Infringed.InventorySystem
{
    public class ItemPickable : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemData _data;

        public Item GetItem()
        {
            return new Item(_data);
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
