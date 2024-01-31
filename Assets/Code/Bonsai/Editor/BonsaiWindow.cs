using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bonsai.Designer.User
{
    public class BonsaiWindow : Bonsai.Designer.BonsaiWindow
    {
        [MenuItem("Window/Bonsai Designer User")]
        static void CreateWindow()
        {
            CreateWindow<BonsaiWindow>();
        }

        protected override void Init()
        {
            Init(new BonsaiEditor());
        }
    }
}
