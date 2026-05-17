/*
 * Description: A debug action that prints to the console.
 */
using Service.Core;
using Service.Framework.Goals;
using UnityEngine;

[Submenu("Debug/Initialize Print")]
public class DebugPrintAction : ObjectiveAction
{
    [SerializeField]
    private string textToPrint;

    public override void InitializeAction()
    {
        if (textToPrint == string.Empty)
        { 
            Debug.Log($"Completed action on {gameObject.name}", gameObject);
        }
        else
        {
            Debug.Log(textToPrint);
        }
        SetComplete();
    }
}
