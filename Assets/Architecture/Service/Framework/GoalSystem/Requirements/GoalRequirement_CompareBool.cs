/*
 * Description: Compares a bool value.  
 *          Useful for more broadly checking completion statuses or other conditionals
 *          from another script or system that may handle a mission logic, etc.
 */
using Service.Framework.GoalManagement;
using System;
using UnityEngine;

namespace Service.Framework.Goals
{
    [Serializable]
    public class GoalRequirement_CompareBool : GoalRequirement
    {
        [Tooltip("The key that is stored in the Blackboard." +
            "Important to make sure the text typed in here is the exact same as it is in the Blackboard's database.")]
        [SerializeField]
        private string key;

        [SerializeField]
        private bool comparisonValue = true;

        public override bool IsRequirementMet(Goal goalToCheck)
        {
            if (key == null)
            {
                Debug.LogError("Please provide a key in the inspector to compare the blackboard bool.");
                return false;
            }
            //the condition is met
            if (GoalManager.Instance.BlackBoard.GetBoolValue(key) == comparisonValue)
            {                
                return true;
            }
            return false;
        }
    }
}