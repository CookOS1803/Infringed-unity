using System.Collections;
using System.Collections.Generic;
using Infringed.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Infringed.InventorySystem.UI
{
    public class UIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ItemData _item;
        private Canvas _canvas;
        private Transform _player;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Image _image;
        private Vector2 _initialPosition;
        private ItemSlot _slot;
        private ItemDescription _itemDescription;
        public Transform Parent => _slot.transform;
        public int Index => _slot.Index;
        public Inventory Inventory { get; set; }

        [Inject]
        private void _SetPlayer(PlayerController controller)
        {
            _player = controller.transform;
        }

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _image = GetComponent<Image>();
            _slot = GetComponentInParent<ItemSlot>();
            _itemDescription = GetComponentInChildren<ItemDescription>();
            _itemDescription.gameObject.SetActive(false);

            _initialPosition = _rectTransform.anchoredPosition;
        }

        public void SetItem(ItemData data)
        {
            _item = data;
            _image.enabled = true;
            _image.sprite = data.Sprite;

            _itemDescription.UpdateInfo(data.Name, data.Description);
        }

        public void UnsetItem()
        {
            _item = null;
            _image.enabled = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.SetParent(Parent.parent);
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.SetParent(Parent);

            if (UserRaycaster.IsBlockedByUI())
            {
                _canvasGroup.blocksRaycasts = true;
                _rectTransform.anchoredPosition = _initialPosition;
                return;
            }
            _rectTransform.anchoredPosition = _initialPosition;
            _canvasGroup.blocksRaycasts = true;

            RaycastHit hit;
            Vector3 spawnPosition;

            if (Physics.Raycast(_player.position + _player.up, _player.forward + _player.up, out hit, 1f))
            {
                spawnPosition = hit.point;
            }
            else
            {
                spawnPosition = _player.position + _player.forward + _player.up;
            }

            Instantiate(_item.Prefab, spawnPosition, Quaternion.identity);
            Inventory[Index] = null;

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _itemDescription.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _itemDescription.gameObject.SetActive(false);
        }
    }
}
