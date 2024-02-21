using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Infringed.Editor
{
    public static class UserMenuItems
    {
        [MenuItem("Assets/Create/Bonsai/Task", false, 1)]
        public static void CreateTask()
        {
            CreateNode("a Task", "NewTask", "Task");
        }

        [MenuItem("Assets/Create/Bonsai/Decorator", false, 1)]
        public static void CreateDecorator()
        {
            CreateNode("a Decorator", "NewDecorator", "Decorator");
        }

        private static void CreateNode(string name, string defaultFileName, string className)
        {
            Zenject.Internal.ZenMenuItems.AddCSharpClassTemplate(name, defaultFileName,
                  "using UnityEngine;"
                + "\nusing Bonsai;"
                + "\nusing Bonsai.Core;"
                + "\n"
                + "\nnamespace Infringed.AI.BTree"
                + "\n{"
                + "\n    [BonsaiNode(\"" + className + "s/\")]"
                + "\n    public class CLASS_NAME : " + className
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