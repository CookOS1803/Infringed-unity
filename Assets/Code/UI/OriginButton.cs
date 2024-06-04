using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.UI
{
    public enum Origin { Thief, Mage }

    public class OriginButton : MonoBehaviour
    {
        [SerializeField] private Origin _origin;

        public void SetOrigin()
        {
            PlayerPrefs.SetInt("Origin", (int)_origin);
        }
    }
}
