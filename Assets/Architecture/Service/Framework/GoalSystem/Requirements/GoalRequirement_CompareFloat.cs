/*
 * Description: Compares a float value
 */
using Service.Framework.GoalManagement;
using System;
using UnityEngine;

namespace Service.Framework.Goals
{
    [Serializable]
    public class GoalRequirement_CompareFloat : GoalRequirement
    {
        [SerializeField]
        private ComparisonOptions comparisonOptions;

        [SerializeField]
        private float valueToCompare;

        [Tooltip("The key that is stored in the Blackboard." +
            "Important to make sure the text typed in here is the exact same as it is in the Blackboard's database.")]
        [SerializeField]
        private string key;

        public override bool IsRequirementMet(Goal goalToCheck)
        {
            if (key == null)
            {
                Debug.LogError("Please provide a key in the inspector to compare the blackboard float.");
                return false;
            }
            float blackboardValue = GoalManager.Instance.BlackBoard.GetFloatValue(key);

            //the possible conditions to meet
            switch (comparisonOptions)
            {
                case ComparisonOptions.GreaterThan:
                    return blackboardValue > valueToCompare;
                case ComparisonOptions.LessThan:
                    return blackboardValue < valueToCompare;
                case ComparisonOptions.LessThanOrEqual:
                    return blackboardValue <= valueToCompare;
                case ComparisonOptions.GreaterThanOrEqual:
                    return blackboardValue >= valueToCompare;
                case ComparisonOptions.EqualTo:
                    return blackboardValue == valueToCompare;
            }
            return false;
        }
    }
}