using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : IEnumerable
{
    private Item[] items;
    private int _selectedSlot = 0;
    public event Action onChange;
    public event Action onSlotSelection;
    
    public int size => items.Length;
    public int selectedSlot
    {
        get => _selectedSlot;
        set
        {
            _selectedSlot = value;

            onSlotSelection?.Invoke();
        }
    }
    public Item this[int i]
    {
        get => items[i];
        set
        {
            items[i] = value;

            onChange?.Invoke();
        }
    }

    public Inventory()
    {
        items = new Item[10];
    }

    public void Add(Item newItem)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = newItem;
                onChange?.Invoke();

                return;
            }
        }

        Debug.LogError("Inventory is full");
    }

    public void SwapSlots(int first, int second)
    {
        if (first == second)
            return;

        if (!HasIndex(first) || !HasIndex(second))
        {
            Debug.LogError("Index is out of range");
            return;
        }

        Item tempItem = items[first];
        items[first] = items[second];
        items[second] = tempItem;

        onChange?.Invoke();
    }

    public IEnumerator GetEnumerator()
    {
        foreach (Item i in items)
        {
            yield return i;
        }
    }

    public bool HasIndex(int i)
    {
        return i >= 0 && i < items.Length;
    }

    public void UseItem(ItemAction.Context context)
    {
        if (items[selectedSlot] == null)
            return;

        ItemAction action = items[selectedSlot].data.action;

        if (action != null)
        {
            action.Use(context);
            items[selectedSlot] = null;

            onChange?.Invoke();
        }
    }

    public bool IsFull()
    {
        foreach (var i in items)
        {
            if (i == null)
                return false;
        }

        return true;
    }
}
