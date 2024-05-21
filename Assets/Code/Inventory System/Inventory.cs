using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.InventorySystem
{
    [System.Serializable]
    public class Inventory
    {
        public event Action<Item> OnItemAdd;

        [field: SerializeField, Min(1)] public int Width { get; private set; } = 10;
        [field: SerializeField, Min(1)] public int Height { get; private set; } = 10;
        private HashSet<Item> _items = new();
        private HashSet<Vector2Int> _freePositions;

        public Inventory()
        {
            _freePositions = new(Width * Height);

            var current = new Vector2Int(0, 0);

            while (current.x < Width)
            {
                while (current.y < Height)
                {
                    _freePositions.Add(current);

                    current.y++;
                }

                current.y = 0;
                current.x++;
            }
        }

        public void AddItem(ItemData itemData)
        {
            if (itemData.Width > Width || itemData.Height > Height)
            {
                Debug.LogError("Item is bigger than the inventory");
                return;
            }

            var bottomLeft = new Vector2Int(0, itemData.Height - 1);
            var topRight = new Vector2Int(itemData.Width - 1, 0);

            if (!_TryToFit(ref bottomLeft, ref topRight))
            {
                bottomLeft = new Vector2Int(0, itemData.Width - 1);
                topRight = new Vector2Int(itemData.Height - 1, 0);

                if (!_TryToFit(ref bottomLeft, ref topRight))
                {
                    Debug.LogError("Item doesn't fit in the inventory");
                    return;
                }
            }

            _Allocate(bottomLeft, topRight);

            var newItem = new Item(itemData)
            {
                BottomLeftPosition = bottomLeft,
                TopRightPosition = topRight
            };
            _items.Add(newItem);

            OnItemAdd?.Invoke(newItem);
        }

        public void RemoveItem(Item item)
        {
            _Deallocate(item.BottomLeftPosition, item.TopRightPosition);

            _items.Remove(item);
        }

        private void _Allocate(Vector2Int bottomLeft, Vector2Int topRight)
        {
            _ActionForArea(bottomLeft, topRight, point => _freePositions.Remove(point));
        }
        
        private void _Deallocate(Vector2Int bottomLeft, Vector2Int topRight)
        {
            _ActionForArea(bottomLeft, topRight, point => _freePositions.Add(point));
        }

        private void _ActionForArea(Vector2Int bottomLeft, Vector2Int topRight, Func<Vector2Int, bool> func)
        {
            var current = new Vector2Int(bottomLeft.x, topRight.y);

            while (current.x <= topRight.x)
            {
                while (current.y <= bottomLeft.y)
                {
                    func(current);

                    current.y++;
                }

                current.y = topRight.y;
                current.x++;
            }
        }

        private bool _TryToFit(ref Vector2Int bottomLeft, ref Vector2Int topRight)
        {
            var initialLeftX = bottomLeft.x;
            var initialRightX = topRight.x;

            while (bottomLeft.y < Height)
            {
                while (topRight.x < Width)
                {
                    if (_Fits(bottomLeft, topRight))
                        return true;

                    bottomLeft.x++;
                    topRight.x++;
                }

                bottomLeft.x = initialLeftX;
                topRight.x = initialRightX;

                bottomLeft.y++;
                topRight.y++;
            }

            return false;
        }

        private bool _Fits(Vector2Int bottomLeft, Vector2Int topRight)
        {
            var current = new Vector2Int(bottomLeft.x, topRight.y);

            while (current.x <= topRight.x)
            {
                while (current.y <= bottomLeft.y)
                {
                    if (!_freePositions.Contains(current))
                        return false;

                    current.y++;
                }

                current.y = topRight.y;
                current.x++;
            }

            return true;
        }
    }
}
