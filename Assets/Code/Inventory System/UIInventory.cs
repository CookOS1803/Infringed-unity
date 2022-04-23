using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    public Inventory inventory { get; private set; }
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Color defaultSlotColor;
    [SerializeField] private Color selectedSlotColor;
    
    void Start()
    {
        for (int i = 0; i < inventory.size; i++)
        {
            Transform child = transform.GetChild(i);
            child.GetComponent<ItemSlot>().index = i;
            child.GetComponent<RawImage>().color = defaultSlotColor;
        }

        transform.GetChild(inventory.selectedSlot).GetComponent<RawImage>().color = selectedSlotColor;
    }

    public void SetInventory(Inventory newInventory)
    {
        inventory = newInventory;
        RefreshInventory();
        inventory.onChange += (object sender, System.EventArgs e) => RefreshInventory();
    }

    public void RefreshInventory()
    {
        for (int i = 0; i < inventory.size; i++)
        {
            ItemSlot slot = transform.GetChild(i).GetComponent<ItemSlot>();
            
            if (inventory[i] == null)
            {
                slot.DestroyItem();
            }
            else
            {
                slot.SetItem(itemPrefab, inventory[i]);
            }
        }
    }

    public void SelectSlot(int index)
    {
        if (index == inventory.selectedSlot)
            return;

        transform.GetChild(inventory.selectedSlot).GetComponent<RawImage>().color = defaultSlotColor;
        transform.GetChild(index).GetComponent<RawImage>().color = selectedSlotColor;

        inventory.selectedSlot = index;
    }
}
