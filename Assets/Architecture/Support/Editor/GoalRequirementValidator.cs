//TODO: Probably deleting this script.

#if UNITY_EDITOR
using Service.Framework.Goals;
using UnityEditor;
using UnityEngine;

namespace Support.Editor
{
    public static class GoalRequirementValidator
    {
        [MenuItem("Tools/Validate Goal Requirements")]
        public static void Validate()
        {
            Goal[] allGoals = Object.FindObjectsByType<Goal>(FindObjectsSortMode.None);
            int brokenRefCount = 0;

            foreach (Goal goal in allGoals)
            {
                SerializedObject referencedGoal = new SerializedObject(goal);
                SerializedProperty requirementsList = referencedGoal.FindProperty(nameof(goal.Requirements));

                if (requirementsList == null || !requirementsList.isArray)
                {
                    continue;
                }

                for (int i = 0; i < requirementsList.arraySize; i++)
                {
                    SerializedProperty element = requirementsList.GetArrayElementAtIndex(i);

                    if (element.managedReferenceValue == null && !string.IsNullOrEmpty(element.managedReferenceFullTypename))
                    {
                        brokenRefCount++;

                        Debug.LogError($"Broken GoalRequirement reference detected at index {i}", goal.gameObject);
                    }
                }
            }
            Debug.Log($"Validation complete.  Broken references found: {brokenRefCount}");
        }
    }
}
#endif