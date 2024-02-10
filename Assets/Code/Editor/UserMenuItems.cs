using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Infringed.Editor
{
    public static class UserMenuItems
    {
        [MenuItem("Assets/Create/Bonsai/Task", false, 1)]
        public static void CreateMemoryPoolInstaller()
        {
            Zenject.Internal.ZenMenuItems.AddCSharpClassTemplate("a Task", "NewTask",
                  "using UnityEngine;"
                + "\nusing Bonsai;"
                + "\nusing Bonsai.Core;"
                + "\n"
                + "\nnamespace Infringed.AI"
                + "\n{"
                + "\n    [BonsaiNode(\"Tasks/\")]"
                + "\n    public class CLASS_NAME : Task"
                + "\n    {"
                + "\n        public override Status Run()"
                + "\n        {"
                + "\n            return Status.Success;"
                + "\n        }"
                + "\n        "
                + "\n    }"
                + "\n}"
                + "\n");
        }
    }
}