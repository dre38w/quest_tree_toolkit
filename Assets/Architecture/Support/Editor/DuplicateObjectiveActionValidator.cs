/*
 * Description: Validates the action ID ownership via Inspector selection changes
 */

#if UNITY_EDITOR
using Service.Framework.Goals;
using UnityEditor;
using UnityEngine;

namespace Support.Editor
{
    [InitializeOnLoad]
    public static class DuplicateObjectiveActionValidator
    {
        static DuplicateObjectiveActionValidator()
        {
            Selection.selectionChanged += ValidateSelection;
        }

        /// <summary>
        /// Validate ownership via the selected game object
        /// </summary>
        private static void ValidateSelection()
        {
            //check all selected objects and their children
            foreach (GameObject selectedObject in Selection.gameObjects)
            {
                ObjectiveAction[] actions = selectedObject.GetComponentsInChildren<ObjectiveAction>(true);

                foreach (ObjectiveAction action in actions)
                {
                    //skip the objects that aren't objective actions
                    if (action == null)
                    {
                        continue;
                    }
                    //validate the ownership
                    action.ValidateInspectorIdOwnership();
                }
            }
        }
    }
}
#endif