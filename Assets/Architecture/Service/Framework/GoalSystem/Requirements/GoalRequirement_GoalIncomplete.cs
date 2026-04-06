/*
 * Description: Checks if a goal is incomplete
 */
using Service.Framework.GoalManagement;
using System;
using UnityEngine;

namespace Service.Framework.Goals
{
    [Serializable]
    public class GoalRequirement_GoalIncomplete : GoalRequirement
    {
        [SerializeField]
        public GoalID goalID;

        public override bool IsRequirementMet(Goal goalToCheck)
        {
            return !GoalManager.Instance.GetGoal(goalID).IsComplete();
        }
    }
}