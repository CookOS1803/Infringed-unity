using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infringed.Math;
using Infringed.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Infringed.InventorySystem.UI
{
    public class UIInventory : MonoBehaviour
    {
        [SerializeField] private PlayerController _player;
        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private ItemGhost _itemGhost;
        [SerializeField] private RectTransform _background;
        [SerializeField] private Vector2 _backgroundAdditionalSize;
        [Zenject.Inject] private UIItem.Factory _itemFactory;
        private PlayerInput _input;
        private Graphic[] _graphics;
        private Dictionary<Item, UIItem> _spawnedItems = new();

        private void Awake()
        {
            _input = GetComponent<PlayerInput>();

            _player.Inventory.OnItemAdd += _OnItemAdd;
            _player.Inventory.OnItemRemove += _OnItemRemove;
            _input.actions["OpenClose"].performed += _SwitchGraphicsStatus;
        }

        private void OnDestroy()
        {
            _player.Inventory.OnItemAdd -= _OnItemAdd;
            _player.Inventory.OnItemRemove -= _OnItemRemove;
            _input.actions["OpenClose"].performed -= _SwitchGraphicsStatus;
        }

        private void Start()
        {
            _itemGhost.gameObject.SetActive(false);

            _MakeGrid();

            _graphics = GetComponentsInChildren<Graphic>(includeInactive: true);
            _SwitchGraphicsStatus(default);
        }

        public Vector2 GetItemPosition(Rectangle rectangle)
        {
            var bottomLeft = _GetGridChild(rectangle.bottomLeft);
            var topRight = _GetGridChild(rectangle.topRight);

            var diag = topRight.anchoredPosition - bottomLeft.anchoredPosition;
            var diagLength = diag.magnitude;

            return bottomLeft.anchoredPosition + diag.normalized * (diagLength / 2f);
        }

        public void DropItem(Item item)
        {
            if (!_player.Inventory.Contains(item))
            {
                Debug.LogError("Can't remove item because it's not in the inventory");
                return;
            }

            var position = _player.transform.position;
            var up = _player.transform.up;
            var forward = _player.transform.forward;
            Vector3 spawnPosition;

            if (Physics.Raycast(position + up, forward + up, out var hit, 1f))
            {
                spawnPosition = hit.point;
            }
            else
            {
                spawnPosition = position + forward + up;
            }

            Instantiate(item.Data.Prefab, spawnPosition, Quaternion.identity);
            _player.Inventory.RemoveItem(item);
        }

        private void _SwitchGraphicsStatus(InputAction.CallbackContext context)
        {
            foreach (var g in _graphics)
            {
                g.enabled = !g.enabled;
            }
        }

        private void _MakeGrid()
        {
            _grid.constraintCount = _player.Inventory.Width;

            var height = _player.Inventory.Height;
            var width = _player.Inventory.Width;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var slotObject = Instantiate(_slotPrefab, _grid.transform);
                    var slot = slotObject.GetComponent<InventorySlot>();
                    slot.GridPosition = new Vector2Int(x, y);
                }
            }

            var size = _background.sizeDelta;
            size.x = width * _grid.cellSize.x + (width - 1) * _grid.spacing.x + _backgroundAdditionalSize.x;
            size.y = height * _grid.cellSize.y + (height - 1) * _grid.spacing.y + _backgroundAdditionalSize.y;
            _background.sizeDelta = size;
        }

        private void _OnItemAdd(Item item)
        {
            var uiItem = _itemFactory.Create();
            uiItem.Item = item;
            uiItem.ItemGhost = _itemGhost;
            uiItem.Inventory = _player.Inventory;
            uiItem.UIInventory = this;
            uiItem.Input = _input;

            _spawnedItems.Add(item, uiItem);

            uiItem.RectTransform.anchoredPosition = GetItemPosition(item.Rectangle);

            var dx = item.Rectangle.DeltaX;
            var dy = item.Rectangle.DeltaY;

            var size = uiItem.RectTransform.sizeDelta;
            size.x = (dx + 1) * _grid.cellSize.x + dx * _grid.spacing.x;
            size.y = (dy + 1) * _grid.cellSize.y + dy * _grid.spacing.y;
            uiItem.RectTransform.sizeDelta = size;

            if (item.IsRotated())
            {
                uiItem.RectTransform.rotation = Quaternion.Euler(0f, 0f, -90f);
                uiItem.RectTransform.sizeDelta = new Vector2(size.y, size.x);
            }
        }

        private void _OnItemRemove(Item item)
        {
            if (!_spawnedItems.ContainsKey(item))
            {
                Debug.LogError("Failed to remove item from UIInventory");
                return;
            }

            var uiItem = _spawnedItems[item];
            _spawnedItems.Remove(item);
            
            uiItem.Dispose();
        }

        private RectTransform _GetGridChild(Vector2Int point)
        {
            var i = point.x + point.y * _player.Inventory.Width;

            return _grid.transform.GetChild(i) as RectTransform;
        }
    }
}
