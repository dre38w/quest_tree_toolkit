/*
 * Description: Compares a vector3's value
 */
using Service.Framework.GoalManagement;
using System;
using UnityEngine;

namespace Service.Framework.Goals
{
    [Serializable]
    public class GoalRequirement_CompareVector : GoalRequirement
    {
        [SerializeField]
        private ComparisonOptions comparisonOptions;

        [SerializeField]
        private Vector3 valueToCompare;

        [Tooltip("The key that is stored in the Blackboard." +
            "Important to make sure the text typed in here is the exact same as it is in the Blackboard's database.")]
        [SerializeField]
        private string key;

        public override bool IsRequirementMet(Goal goalToCheck)
        {
            if (key == null)
            {
                Debug.LogError("Please provide a key in the inspector to compare the blackboard Vector3.");
                return false;
            }
            float blackboardValue = GoalManager.Instance.BlackBoard.GetVector3Value(key).sqrMagnitude;

            //possible conditions to meet
            switch (comparisonOptions)
            {
                case ComparisonOptions.GreaterThan:
                    return blackboardValue > valueToCompare.sqrMagnitude;
                case ComparisonOptions.LessThan:
                    return blackboardValue < valueToCompare.sqrMagnitude;
                case ComparisonOptions.LessThanOrEqual:
                    return blackboardValue <= valueToCompare.sqrMagnitude;
                case ComparisonOptions.GreaterThanOrEqual:
                    return blackboardValue >= valueToCompare.sqrMagnitude;
                case ComparisonOptions.EqualTo:
                    return blackboardValue == valueToCompare.sqrMagnitude;
            }
            return false;
        }
    }
}