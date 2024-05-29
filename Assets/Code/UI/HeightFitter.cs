using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.UI
{
    public class HeightFitter : MonoBehaviour
    {
        public RectTransform RectTransform => transform as RectTransform;

        private void Update()
        {
            var height = 0f;

            foreach (RectTransform c in transform)
            {
                height += c.sizeDelta.y;
            }

            RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, height);
        }
    }
}
