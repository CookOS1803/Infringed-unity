using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.Quests
{
    [System.Serializable]
    public class IndexedLabel
    {
        public event Action<IndexedLabel> OnClicked;
        public string Label;
        public int Index;

        public void Click()
        {
            OnClicked?.Invoke(this);
        }

        public void ClearCallback()
        {
            OnClicked = null;
        }
    }
}
