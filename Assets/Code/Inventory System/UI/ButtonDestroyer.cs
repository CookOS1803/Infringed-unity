using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Infringed.InventorySystem.UI
{
    public class ButtonDestroyer : MonoBehaviour
    {
        [SerializeField] private Button _button;
        private TestEssentia _essentia;
        

        private void Start()
        {
            _essentia = FindObjectOfType<TestEssentia>();
        }

        private void Update()
        {
            _button.interactable = _essentia._essentia > 0;
        }

        public void DestroyButton()
        {
            Destroy(_button.gameObject);
        }
    }
}
