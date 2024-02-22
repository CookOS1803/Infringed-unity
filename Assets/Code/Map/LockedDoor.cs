using Infringed.InventorySystem;
using Infringed.Player;
using UnityEngine;

namespace Infringed.Map
{
    public class LockedDoor : Door
    {
        [SerializeField] private ItemData _key;

        public override void Interact(PlayerController user)
        {
            if (IsClosed)
            {
                var inventory = user?.Inventory;

                if (inventory != null)
                {
                    foreach (Item i in inventory)
                    {
                        if (i?.Data == _key)
                        {
                            _Open();
                            break;
                        }
                    }
                }
            }
            else
            {
                _Close();
            }
        }
    }
}
