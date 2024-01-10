using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonsai.Core;
using Bonsai;

[BonsaiNode("Tasks/Knight/")]
public class PrintTask : Task
{
    public override Status Run()
    {
        Debug.Log(Tree.actor);
        return Status.Running;
    }

    
}
