using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infringed.Quests
{
    [System.Serializable]
    public class DialogueChoice
    {
        [field: SerializeField, TextArea] public string Text { get; set; }
        [field: SerializeField] public List<IndexedLabel> Options;
    }
}
