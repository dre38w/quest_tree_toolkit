/*
 * Description: Checks if a goal is complete
 */
using Service.Framework.GoalManagement;
using System;
using UnityEngine;

namespace Service.Framework.Goals
{
    [Serializable]
    public class GoalRequirement_GoalComplete : GoalRequirement
    {
        [SerializeField]
        private GoalID goalID;

        public override bool IsRequirementMet(Goal goalToCheck)
        {
            return GoalManager.Instance.GetGoal(goalID).IsComplete();
        }
    }
}