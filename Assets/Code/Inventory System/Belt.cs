using System;
using System.Collections;
using System.Collections.Generic;
using Infringed.Actions;
using UnityEngine;

namespace Infringed.InventorySystem
{
    public class Belt : IEnumerable
    {
        private ItemInstance[] _items;
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
        public ItemInstance this[int i]
        {
            get => _items[i];
            set
            {
                _items[i] = value;

                OnChange?.Invoke();
            }
        }
        public ItemInstance SelectedItem => _items[SelectedSlot];

        public Belt(Inventory inventory)
        {
            _items = new ItemInstance[10];

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
            return !_items[index].Data.IsInventoryItem() || Inventory.ContainsData(_items[index].Data);
        }

        public bool ContainsItem(ItemData data)
        {
            foreach (var item in _items)
            {
                if (item?.Data == data)
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

            if (SelectedItem.Data.Action != null)
            {
                if (SelectedItem.CurrentCooldown > 0)
                    return;

                if (SelectedItem.Data.IsInventoryItem() && SelectedItem.Data.Consumable)
                {
                    if (!Inventory.Consume(SelectedItem.Data))
                        return;

                    SelectedItem.Data.Action.Use(context);

                    OnChange?.Invoke();
                }
                else
                    SelectedItem.Data.Action.Use(context);

                SelectedItem.LastUsedTime = Time.time;
            }
        }
    }
}