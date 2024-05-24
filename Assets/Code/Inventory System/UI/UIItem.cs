using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using Infringed.Math;
using UnityEngine.InputSystem;

namespace Infringed.InventorySystem.UI
{
    public class UIItem : MonoBehaviour, IPoolable<IMemoryPool>, System.IDisposable,
                          IInitializePotentialDragHandler, IPointerClickHandler, IBeginDragHandler,
                          IDragHandler, IEndDragHandler
    {
        private Item _item;
        private CanvasGroup _canvasGroup;
        private Canvas _canvas;
        private Rectangle _dragRectangle;
        private InventorySlot _slot;
        [field: SerializeField] public Image Image { get; private set; }
        public RectTransform RectTransform => transform as RectTransform;
        public Item Item
        {
            get => _item;
            set
            {
                _item = value;

                Image.sprite = _item?.Data.InventorySprite;
            }
        }
        public ItemGhost ItemGhost { get; set; }
        public Inventory Inventory { get; set; }
        public UIInventory UIInventory { get; set; }
        public PlayerInput Input { get; set; }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvas = GetComponentInParent<Canvas>();
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            ItemGhost.Clone(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            ItemGhost.gameObject.SetActive(true);
            _canvasGroup.blocksRaycasts = false;
            _dragRectangle = Item.Rectangle;

            _MatchItemToCursor(eventData.position);

            Input.actions["RotateItem"].performed += _RotateItem;
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;

            _slot = eventData.hovered.Select(obj => obj.GetComponent<InventorySlot>())
                                     .FirstOrDefault(slot => slot != null);

            _CalculateItemGhostPosition();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Inventory.MoveItem(Item, ItemGhost.Rectangle);
            RectTransform.rotation = ItemGhost.RectTransform.rotation;
            RectTransform.anchoredPosition = ItemGhost.RectTransform.anchoredPosition;

            ItemGhost.gameObject.SetActive(false);
            _canvasGroup.blocksRaycasts = true;

            Input.actions["RotateItem"].performed -= _RotateItem;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!eventData.dragging && eventData.button == PointerEventData.InputButton.Right)
                UIInventory.DropItem(this);
        }

        private void _RotateItem(InputAction.CallbackContext context)
        {
            var beforeDragStatus = Item.GetRotationStatus();

            if (beforeDragStatus == Item.RotationStatus.Square)
                return;

            var currentStatus = Item.GetRotationStatus(_dragRectangle);

            if (currentStatus == Item.RotationStatus.Invalid)
            {
                Debug.LogError("Invalid rectangle");

                return;
            }

            if (currentStatus == Item.RotationStatus.NonRotated)
            {
                var x = _dragRectangle.bottomLeft.x + Item.Data.Height - 1;
                var y = _dragRectangle.bottomLeft.y - Item.Data.Width + 1;
                _dragRectangle.topRight = new Vector2Int(x, y);

                RectTransform.rotation = Quaternion.Euler(0f, 0f, -90f);
            }
            else
            {
                var x = _dragRectangle.bottomLeft.x + Item.Data.Width - 1;
                var y = _dragRectangle.bottomLeft.y - Item.Data.Height + 1;
                _dragRectangle.topRight = new Vector2Int(x, y);

                RectTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }

            _CalculateItemGhostPosition();
        }

        private void _CalculateItemGhostPosition()
        {   
            if (_slot == null)
                return;
            
            var delta = _dragRectangle.topRight - _dragRectangle.bottomLeft;
            var rectangle = new Rectangle(_slot.GridPosition, _slot.GridPosition + delta);
            rectangle = Inventory.GetNearestValidRectangle(rectangle);

            var doesntFit = !(Inventory.FitsExcluded(rectangle, Item.Rectangle));

            if (doesntFit)
                return;

            ItemGhost.Rectangle = rectangle;
            ItemGhost.RectTransform.anchoredPosition = UIInventory.GetItemPosition(rectangle);
            ItemGhost.RectTransform.rotation = RectTransform.rotation;
        }

        private void _MatchItemToCursor(Vector2 cursorPos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle
            (
                transform.parent as RectTransform,
                cursorPos,
                _canvas.worldCamera,
                out var pos
            );

            var offsetX = -RectTransform.rect.width;
            var offsetY = RectTransform.rect.height * 0.5f;
            var offset = new Vector2(offsetX, offsetY);
            RectTransform.anchoredPosition = pos - offset;
        }

        #region Memory pool
        private IMemoryPool _pool;

        public void OnSpawned(IMemoryPool pool)
        {
            _pool = pool;
        }

        public void OnDespawned()
        {
            _pool = null;

            Item = null;
            ItemGhost = null;
            Inventory = null;
            UIInventory = null;
            Input = null;
            _slot = null;
            _dragRectangle = default;
        }

        public void Dispose()
        {
            _pool.Despawn(this);
        }

        public class Factory : PlaceholderFactory<UIItem> { }
        #endregion
    }
}
