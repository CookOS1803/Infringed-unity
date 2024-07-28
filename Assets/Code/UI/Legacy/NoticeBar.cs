using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.Legacy
{
    [System.Obsolete]
    public class NoticeBar : MonoBehaviour
    {
        private SpriteRenderer sprite;
        private SpriteRenderer parentSprite;
        private VisionController enemy;

        void Start()
        {
            sprite = GetComponent<SpriteRenderer>();
            parentSprite = transform.parent.GetComponent<SpriteRenderer>();
            enemy = GetComponentInParent<VisionController>();

            enemy.onNoticeClockChange += UpdateBar;
            enemy.onNoticeClockReset += HideBar;
        }

        void Update()
        {

        }

        void UpdateBar()
        {
            sprite.enabled = true;
            parentSprite.enabled = true;
            sprite.size = new Vector2(enemy.normalizedNoticeClock, sprite.size.y);
        }

        void HideBar()
        {
            sprite.enabled = false;
            parentSprite.enabled = false;
        }
    }
}
