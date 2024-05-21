using System.Collections;
using System.Collections.Generic;
using Infringed.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Infringed.InventorySystem.UI
{
    public class UIBeltItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ItemData _item;
        [Inject] private PlayerController _player;
        private Canvas _canvas;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Image _image;
        private Vector2 _initialPosition;
        private BeltSlot _slot;
        private ItemDescription _itemDescription;
        public Transform Parent => _slot.transform;
        public int Index => _slot.Index;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _image = GetComponent<Image>();
            _slot = GetComponentInParent<BeltSlot>();
            _itemDescription = GetComponentInChildren<ItemDescription>();

            _initialPosition = _rectTransform.anchoredPosition;
        }

        private void Start()
        {
            _itemDescription.gameObject.SetActive(false);
        }

        public void SetItem(ItemData data)
        {
            _item = data;
            _image.enabled = true;
            _image.sprite = data.BeltSprite;

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

            Instantiate(_item.Prefab, spawnPosition, Quaternion.identity);
            _player.Belt[Index] = null;

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
