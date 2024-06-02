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
            if (_isClosed)
            {
                var inventory = user?.Inventory;

                if (inventory != null && inventory.ContainsImportantItem(_key))
                {
                    _Open();
                }
            }
            else
            {
                _Close();
            }
        }
    }
}
