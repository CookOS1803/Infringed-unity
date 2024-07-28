using System;
using System.Collections;
using System.Collections.Generic;
using Infringed.Math;
using UnityEngine;

namespace Infringed.InventorySystem
{
    [System.Serializable]
    public class Inventory : IDisposable
    {
        public event Action<InventoryItemInstance> OnItemAdd;
        public event Action<ItemData> OnImportantItemAdd;
        public event Action<ItemData> OnItemAddFailure;
        public event Action<InventoryItemInstance> OnItemRemove;
        public event Action OnSizeChange;

        public AbilitySet AbilitySet { get; set; }
        [field: SerializeField, Min(1)] public int Width { get; private set; } = 10;
        [field: SerializeField, Min(1)] public int Height { get; private set; } = 10;
        [field: SerializeField] public Ability SizeAbility { get; private set; }
        [SerializeField, Min(1)] private int _enhancedWidth = 5;
        [SerializeField, Min(1)] private int _enhancedHeight = 5;
        private HashSet<InventoryItemInstance> _items = new();
        private HashSet<ItemData> _importantItems = new();
        private HashSet<Vector2Int> _freePositions;

        public Inventory()
        {
            _freePositions = new(Width * Height);

            var rectangle = new Rectangle(0, Height - 1, Width - 1, 0);

            _AddFreeRectangle(rectangle);
        }

        public void Awake()
        {
            if (_enhancedWidth < Width || _enhancedHeight < Height)
            {
                Debug.LogError("Enhanced width or height smaller than initial are not supported");
            }

            AbilitySet.OnLearned += _OnLearned;
        }

        public void Dispose()
        {
            AbilitySet.OnLearned -= _OnLearned;
        }

        public bool AddItem(ItemData itemData)
        {
            if (itemData.IsImportantItem())
            {
                _AddImportantItem(itemData);
                return true;
            }

            if (itemData.Width > Width || itemData.Height > Height)
            {
                OnItemAddFailure?.Invoke(itemData);
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
                    OnItemAddFailure?.Invoke(itemData);
                    return false;
                }
            }

            _Allocate(rectangle);

            var newItem = new InventoryItemInstance(itemData)
            {
                Rectangle = rectangle
            };

            var cooldownAbility = AbilitySet.GetAbilityInstance(itemData.CooldownAbility);
            if (cooldownAbility != null && cooldownAbility.IsLearned)
                newItem.Cooldown = itemData.EnhancedCooldown;

            _items.Add(newItem);

            OnItemAdd?.Invoke(newItem);

            return true;
        }

        public void RemoveItem(InventoryItemInstance item)
        {
            _Deallocate(item.Rectangle);

            _items.Remove(item);

            OnItemRemove?.Invoke(item);
        }

        public bool Consume(ItemData consumedItem)
        {
            foreach (var item in _items)
            {
                if (item.Data != consumedItem)
                    continue;

                RemoveItem(item);

                return true;
            }

            return false;
        }

        public bool MoveItem(InventoryItemInstance item, Rectangle newRectangle)
        {
            if (!FitsExcluded(newRectangle, item.Rectangle))
            {
                Debug.LogWarning("Item can't be moved in new position");
                return false;
            }

            _Deallocate(item.Rectangle);
            _Allocate(newRectangle);

            item.Rectangle = newRectangle;

            return true;
        }

        public bool Contains(InventoryItemInstance item)
        {
            return _items.Contains(item);
        }

        public bool ContainsData(ItemData itemData)
        {
            foreach (var item in _items)
            {
                if (item.Data != itemData)
                    continue;

                return true;
            }

            return false;
        }

        public bool ContainsImportantItem(ItemData important)
        {
            return _importantItems.Contains(important);
        }

        public bool Fits(Rectangle rectangle)
        {
            return _FitsIf(rectangle, point => _freePositions.Contains(point));
        }

        public bool FitsExcluded(Rectangle rectangle, Rectangle rectangleExcluded)
        {
            return _FitsIf(rectangle, point => _freePositions.Contains(point) || IsInside(point, rectangleExcluded));
        }

        public int GetItemCount(ItemData data)
        {
            var count = 0;

            foreach (var item in _items)
            {
                if (item.Data == data)
                    count++;
            }

            return count;
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

        private void _OnLearned(AbilityInstance instance)
        {
            if (instance.Ability == SizeAbility)
            {
                var rectangle = new Rectangle(0, _enhancedHeight - 1, _enhancedWidth - 1, Height);
                _AddFreeRectangle(rectangle);

                rectangle = new Rectangle(Width, Height - 1, _enhancedWidth - 1, 0);
                _AddFreeRectangle(rectangle);

                Width = _enhancedWidth;
                Height = _enhancedHeight;

                OnSizeChange?.Invoke();
            }

            foreach (var item in _items)
            {
                if (item.Data.CooldownAbility == instance.Ability)
                    item.Cooldown = item.Data.EnhancedCooldown;
            }
        }

        private void _AddFreeRectangle(Rectangle rectangle)
        {
            var initialX = rectangle.bottomLeft.x;
            var initialY = rectangle.topRight.y;
            var current = new Vector2Int(initialX, initialY);

            while (current.x <= rectangle.topRight.x)
            {
                while (current.y <= rectangle.bottomLeft.y)
                {
                    _freePositions.Add(current);

                    current.y++;
                }

                current.y = initialY;
                current.x++;
            }
        }

        private void _AddImportantItem(ItemData itemData)
        {
            if (_importantItems.Add(itemData))
                OnImportantItemAdd?.Invoke(itemData);
        }

        private static bool _FitsIf(Rectangle rectangle, Predicate<Vector2Int> predicate)
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

        private static void _ActionForArea(Rectangle area, Predicate<Vector2Int> action)
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
