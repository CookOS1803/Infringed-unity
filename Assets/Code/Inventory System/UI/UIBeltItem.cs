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
        [SerializeField] private Image _cooldownImage;
        [Inject] private PlayerController _player;
        private Canvas _canvas;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Vector2 _initialPosition;
        private BeltSlot _slot;
        private ItemDescription _itemDescription;
        public Image Image { get; private set; }
        public Transform Parent => _slot.transform;
        public int Index => _slot.Index;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            Image = GetComponent<Image>();
            _slot = GetComponentInParent<BeltSlot>();
            _itemDescription = GetComponentInChildren<ItemDescription>();

            _initialPosition = _rectTransform.anchoredPosition;
        }

        private void Start()
        {
            _itemDescription.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_item == null)
                return;
            
            var normalizedCooldown = _item.CurrentCooldown / _item.Cooldown;

            _cooldownImage.fillAmount = normalizedCooldown;
        }

        public void SetItem(ItemData data)
        {
            _item = data;
            Image.enabled = true;
            Image.sprite = data.BeltSprite;

            _itemDescription.UpdateInfo(data.Name, data.Description);
        }

        public void UnsetItem()
        {
            _item = null;
            Image.enabled = false;

            _itemDescription.UpdateInfo("", "");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            transform.SetParent(Parent.parent);
            _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        // TTTTTTTTTTTTTTTT
        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            transform.SetParent(Parent);

            _canvasGroup.blocksRaycasts = true;
            _rectTransform.anchoredPosition = _initialPosition;

            if (!UserRaycaster.IsBlockedByUI())
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
