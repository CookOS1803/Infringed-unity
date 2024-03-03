using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Infringed.Editor
{
    public static class UserMenuItems
    {
        [MenuItem("Assets/Create/Bonsai/Task", false, 1)]
        private static void _CreateTask()
        {
            Zenject.Internal.ZenMenuItems.AddCSharpClassTemplate("a Task", "NewTask",
                  "using UnityEngine;"
                + "\nusing Bonsai;"
                + "\nusing Bonsai.Core.User;"
                + "\n"
                + "\nnamespace Infringed.AI.BTree"
                + "\n{"
                + "\n    [BonsaiNode(\"Tasks/\")]"
                + "\n    public class CLASS_NAME : FailableTask"
                + "\n    {"
                + "\n        protected override Status _FailableRun()"
                + "\n        {"
                + "\n            return Status.Success;"
                + "\n        }"
                + "\n        "
                + "\n    }"
                + "\n}"
                + "\n");
        }

        [MenuItem("Assets/Create/Bonsai/Decorator", false, 1)]
        private static void _CreateDecorator()
        {
            _CreateNode("a Decorator", "NewDecorator", "Decorator");
        }

        [MenuItem("Assets/Create/Bonsai/Conditional", false, 1)]
        private static void _CreateConditional()
        {
            Zenject.Internal.ZenMenuItems.AddCSharpClassTemplate("a Conditional", "NewConditional",
                  "using UnityEngine;"
                + "\nusing Bonsai;"
                + "\nusing Bonsai.Core;"
                + "\nusing Bonsai.Core.User;"
                + "\n"
                + "\nnamespace Infringed.AI.BTree"
                + "\n{"
                + "\n    [BonsaiNode(\"Conditional/\")]"
                + "\n    public class CLASS_NAME : ConditionalForFailables"
                + "\n    {"
                + "\n        protected override bool _InvertableCondition()"
                + "\n        {"
                + "\n            return true;"
                + "\n        }"
                + "\n        "
                + "\n    }"
                + "\n}"
                + "\n");
        }

        private static void _CreateNode(string name, string defaultFileName, string className)
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