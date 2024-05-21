using System.Collections;
using System.Collections.Generic;
using Infringed.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Infringed.InventorySystem.UI
{
    public class UIInventory : MonoBehaviour
    {
        [SerializeField] private PlayerController _player;
        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private GameObject _slotPrefab;
        [Zenject.Inject] private UIItem.Factory _itemFactory;

        private void Awake()
        {

        }

        private void OnEnable()
        {
            _player.Inventory.OnItemAdd += _OnItemAdd;
        }

        private void OnDisable()
        {
            _player.Inventory.OnItemAdd -= _OnItemAdd;
        }

        private void Start()
        {
            _grid.constraintCount = _player.Inventory.Width;

            var count = _player.Inventory.Width * _player.Inventory.Height;

            for (int i = 0; i < count; i++)
            {
                Instantiate(_slotPrefab, _grid.transform);
            }
        }

        private RectTransform _GetGridChild(Vector2Int point)
        {
            var i = point.x + point.y * _player.Inventory.Width;

            return _grid.transform.GetChild(i) as RectTransform;
        }

        private void _OnItemAdd(Item item)
        {
            var bottomLeft = _GetGridChild(item.BottomLeftPosition);
            var topRight = _GetGridChild(item.TopRightPosition);

            bottomLeft.GetComponent<RawImage>().color = Color.red;
            topRight.GetComponent<RawImage>().color = Color.red;

            var uiItem = _itemFactory.Create();
            uiItem.Item = item;

            var diag = topRight.anchoredPosition - bottomLeft.anchoredPosition;
            var diagLength = diag.magnitude;
            
            var itemTransform = uiItem.transform as RectTransform;
            itemTransform.anchoredPosition = bottomLeft.anchoredPosition + diag.normalized * (diagLength / 2f);

            var dx = item.TopRightPosition.x - item.BottomLeftPosition.x;
            var dy = item.BottomLeftPosition.y - item.TopRightPosition.y;

            var size = itemTransform.sizeDelta;
            size.x = (dx + 1) * _grid.cellSize.x + dx * _grid.spacing.x;
            size.y = (dy + 1) * _grid.cellSize.y + dy * _grid.spacing.y;
            itemTransform.sizeDelta = size;

            if (item.IsRotated())
            {
                itemTransform.rotation = Quaternion.Euler(0f, 0f, -90f);
                itemTransform.sizeDelta = new Vector2(size.y, size.x);
            }
        }
    }
}
