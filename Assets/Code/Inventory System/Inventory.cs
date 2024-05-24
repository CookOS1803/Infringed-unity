using System;
using System.Collections;
using System.Collections.Generic;
using Infringed.Math;
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

        public bool AddItem(ItemData itemData)
        {
            if (itemData.Width > Width || itemData.Height > Height)
            {
                Debug.LogWarning("Item is bigger than the inventory");
                return false;
            }

            var rectangle = new Rectangle()
            {
                bottomLeft = new Vector2Int(0, itemData.Height - 1),
                topRight = new Vector2Int(itemData.Width - 1, 0)
            };

            if (!_TryToFit(ref rectangle))
            {
                rectangle.bottomLeft = new Vector2Int(0, itemData.Width - 1);
                rectangle.topRight = new Vector2Int(itemData.Height - 1, 0);

                if (itemData.Width == itemData.Height || !_TryToFit(ref rectangle))
                {
                    Debug.LogWarning("Item doesn't fit in the inventory");
                    return false;
                }
            }

            _Allocate(rectangle);

            var newItem = new Item(itemData)
            {
                Rectangle = rectangle
            };
            _items.Add(newItem);

            OnItemAdd?.Invoke(newItem);

            return true;
        }

        public bool RemoveItem(Item item)
        {
            if (!_items.Contains(item))
            {
                Debug.LogError("Can't remove item because it's not in the inventory");
                return false;
            }
            
            _Deallocate(item.Rectangle);

            return _items.Remove(item);
        }

        public bool MoveItem(Item item, Rectangle newRectangle)
        {
            if (!FitsExcluded(newRectangle, item.Rectangle))
            {
                Debug.Log("Item can't be moved in new position");
                return false;
            }

            _Deallocate(item.Rectangle);
            _Allocate(newRectangle);

            item.Rectangle = newRectangle;

            return true;
        }

        public bool Fits(Rectangle rectangle)
        {
            return _FitsIf(rectangle, point => _freePositions.Contains(point));
        }

        public bool FitsExcluded(Rectangle rectangle, Rectangle rectangleExcluded)
        {
            return _FitsIf(rectangle, point => _freePositions.Contains(point) || IsInside(point, rectangleExcluded));
        }

        public Rectangle GetNearestValidRectangle(Rectangle rectangle)
        {
            if (rectangle.bottomLeft.x < 0)
                rectangle.MoveX(-rectangle.bottomLeft.x);
            else if (rectangle.topRight.x >= Width)
                rectangle.MoveX(Width - rectangle.topRight.x - 1);
            
            if (rectangle.bottomLeft.y >= Height)
                rectangle.MoveY(Height - rectangle.bottomLeft.y - 1);
            else if (rectangle.topRight.y < 0)
                rectangle.MoveY(-rectangle.topRight.y);

            return rectangle;
        }

        public static bool IsValid(Vector2Int point)
        {
            return point.x >= 0 && point.y >= 0;
        }

        public static bool IsInside(Vector2Int point, Rectangle rectangle)
        {
            return point.x >= rectangle.bottomLeft.x && point.y <= rectangle.bottomLeft.y &&
                   point.x <= rectangle.topRight.x && point.y >= rectangle.topRight.y;
        }

        private static bool _FitsIf(Rectangle rectangle, Func<Vector2Int, bool> predicate)
        {
            var current = new Vector2Int(rectangle.bottomLeft.x, rectangle.topRight.y);

            while (current.x <= rectangle.topRight.x)
            {
                while (current.y <= rectangle.bottomLeft.y)
                {
                    if (!predicate(current))
                        return false;

                    current.y++;
                }

                current.y = rectangle.topRight.y;
                current.x++;
            }

            return true;
        }

        private void _Allocate(Rectangle area)
        {
            _ActionForArea(area, point => _freePositions.Remove(point));
        }
        
        private void _Deallocate(Rectangle area)
        {
            _ActionForArea(area, point => _freePositions.Add(point));
        }

        private static void _ActionForArea(Rectangle area, Func<Vector2Int, bool> action)
        {
            var current = new Vector2Int(area.bottomLeft.x, area.topRight.y);

            while (current.x <= area.topRight.x)
            {
                while (current.y <= area.bottomLeft.y)
                {
                    action(current);

                    current.y++;
                }

                current.y = area.topRight.y;
                current.x++;
            }
        }

        private bool _TryToFit(ref Rectangle rectangle)
        {
            var initialLeftX = rectangle.bottomLeft.x;
            var initialRightX = rectangle.topRight.x;

            while (rectangle.bottomLeft.y < Height)
            {
                while (rectangle.topRight.x < Width)
                {
                    if (Fits(rectangle))
                        return true;

                    rectangle.MoveX(1);
                }

                rectangle.bottomLeft.x = initialLeftX;
                rectangle.topRight.x = initialRightX;

                rectangle.MoveY(1);
            }

            return false;
        }
    }
}
