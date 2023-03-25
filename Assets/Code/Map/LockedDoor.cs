using UnityEngine;

public class LockedDoor : Door
{
    [SerializeField] private ItemData key;

    public override void Interact(PlayerController user)
    {
        if (isClosed)
        {
            var inventory = user?.inventory;

            if (inventory != null)
            {
                foreach (Item i in inventory)
                {
                    if (i?.data == key)
                    {
                        Open();
                        break;
                    }
                }
            }
        }
        else
        {
            Close();
        }
    }
}