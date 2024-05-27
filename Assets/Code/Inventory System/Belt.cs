using System;
using System.Collections;
using System.Collections.Generic;
using Infringed.Actions;
using UnityEngine;

namespace Infringed.InventorySystem
{
    public class Belt : IEnumerable
    {
        private ItemData[] _items;
        private int _selectedSlot = 0;
        public Inventory Inventory { get; private set; }
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
        public ItemData this[int i]
        {
            get => _items[i];
            set
            {
                _items[i] = value;

                OnChange?.Invoke();
            }
        }
        public ItemData SelectedItem => _items[SelectedSlot];

        public Belt(Inventory inventory)
        {
            _items = new ItemData[10];

            Inventory = inventory;
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

            var tempItem = _items[first];
            _items[first] = _items[second];
            _items[second] = tempItem;

            OnChange?.Invoke();
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var i in _items)
            {
                yield return i;
            }
        }

        public bool HasIndex(int i)
        {
            return i >= 0 && i < _items.Length;
        }

        public bool HasItemsInInventoty(int index)
        {
            return !_items[index].IsInventoryItem() || Inventory.ContainsData(_items[index]);
        }

        public bool ContainsItem(ItemData data)
        {
            foreach (var item in _items)
            {
                if (item == data)
                    return true;
            }

            return false;
        }

        public int GetFirstNullIndex()
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] == null)
                    return i;
            }

            return -1;
        }

        public void UseItem(ItemAction.Context context)
        {
            if (SelectedItem == null)
                return;

            if (SelectedItem.Action != null)
            {
                if (SelectedItem.IsInventoryItem() && SelectedItem.Consumable)
                {
                    if (!Inventory.Consume(SelectedItem))
                        return;

                    SelectedItem.Action.Use(context);

                    OnChange?.Invoke();
                }
                else
                    SelectedItem.Action.Use(context);

            }
        }
    }
}