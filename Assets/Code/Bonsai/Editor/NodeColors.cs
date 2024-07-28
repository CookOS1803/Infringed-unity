using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonsai.Designer.User
{
    [CreateAssetMenu(menuName = "Bonsai/Node Colors", fileName = "Node Colors")]
    public class NodeColors : ScriptableObject
    {
        [field: SerializeField] public Color FailableTaskColor { get; private set; }
        [field: SerializeField] public Color ConditionalForFailablesColor { get; private set; }
        [field: SerializeField] public Color ConditionalForFailablesInvertedColor { get; private set; }
    }
}
