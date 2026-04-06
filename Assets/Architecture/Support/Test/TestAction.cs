using Service.Framework.Goals;
using UnityEngine;

public class TestAction : ObjectiveAction
{
    public bool testBool = false;

    public override void InitializeAction()
    {
        SetComplete();
    }
}
