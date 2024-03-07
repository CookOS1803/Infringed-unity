using System;
using System.Collections;
using System.Collections.Generic;
using Infringed.Actions;
using UnityEngine;

namespace Infringed.InventorySystem
{
    public class Inventory : IEnumerable
    {
        private Item[] _items;
        private int _selectedSlot = 0;
        public event Action OnChange;
        public event Action OnSlotSelection;

        public int Size => _items.Length;
        public int SelectedSlot
        {
            get => _selectedSlot;
            set
            {
                _selectedSlot = value;

                OnSlotSelection?.Invoke();
            }
        }
        public Item this[int i]
        {
            get => _items[i];
            set
            {
                _items[i] = value;

                OnChange?.Invoke();
            }
        }
        public Item SelectedItem => _items[SelectedSlot];

        public Inventory()
        {
            _items = new Item[10];
        }

        public void Add(Item newItem)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] == null)
                {
                    _items[i] = newItem;
                    OnChange?.Invoke();

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

            Item tempItem = _items[first];
            _items[first] = _items[second];
            _items[second] = tempItem;

            OnChange?.Invoke();
        }

        public IEnumerator GetEnumerator()
        {
            foreach (Item i in _items)
            {
                yield return i;
            }
        }

        public bool HasIndex(int i)
        {
            return i >= 0 && i < _items.Length;
        }

        public void UseItem(ItemAction.Context context)
        {
            if (_items[SelectedSlot] == null)
                return;

            var action = _items[SelectedSlot].Data.Action;

            if (action != null)
            {
                action.Use(context);
                _items[SelectedSlot] = null;

                OnChange?.Invoke();
            }
        }

        public bool IsFull()
        {
            foreach (var i in _items)
            {
                if (i == null)
                    return false;
            }

            return true;
        }
    }
}