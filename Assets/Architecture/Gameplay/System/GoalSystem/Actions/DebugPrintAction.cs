/*
 * Description: A debug action that prints to the console.
 */
using Service.Core;
using Service.Framework.Goals;
using UnityEngine;

[Submenu("Debug/Initialize Print")]
public class DebugPrintAction : ObjectiveAction
{
    public override void InitializeAction()
    {
        Debug.Log($"Completed action on {gameObject.name}", gameObject);
        SetComplete();
    }
}
