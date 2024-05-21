using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Infringed.InventorySystem.UI
{
    public class UIItem : MonoBehaviour, IPoolable<IMemoryPool>, System.IDisposable
    {
        private Image _image;
        private Item _item;
        public Item Item
        {
            get => _item;
            set
            {
                _item = value;

                _image.sprite = _item?.Data.InventorySprite;
            }
        }

        private void Awake()
        {
            _image = GetComponent<Image>();
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
        }

        public void Dispose()
        {
            _pool.Despawn(this);
        }

        public class Factory : PlaceholderFactory<UIItem> { }
        #endregion
    }
}
