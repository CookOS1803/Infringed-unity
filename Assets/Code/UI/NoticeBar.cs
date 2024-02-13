using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.AI
{
    public class NoticeBar : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _fillerSprite;
        [SerializeField] private SpriteRenderer _backgroundSprite;
        [SerializeField] private SuspicionController _suspicion;

        private void Update()
        {
            if (_suspicion.NoticeClock > 0f)
                _UpdateBar();
            else
                _HideBar();
        }

        private void _UpdateBar()
        {
            _fillerSprite.enabled = true;
            _backgroundSprite.enabled = true;
            _fillerSprite.size = new Vector2(_suspicion.NoticeClock / _suspicion.NoticeTime, _fillerSprite.size.y);
        }

        private void _HideBar()
        {
            _fillerSprite.enabled = false;
            _backgroundSprite.enabled = false;
        }
    }
}
