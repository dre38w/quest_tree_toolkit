/*
 * Description: Automatically triggers a specified goal to start
 */
using Service.Framework.GoalManagement;
using UnityEngine;

namespace Service.Framework.Goals
{
    public class TriggerGoalAction : ObjectiveAction
    {
        /// <summary>
        /// Use an array in the event we want to trigger multiple goals
        /// </summary>
        [SerializeField]
        private GoalID[] goalIDsToTrigger;

        public override void InitializeAction()
        {
            for (int i = 0; i < goalIDsToTrigger.Length; i++)
            {
                GoalManager.Instance.GetGoal(goalIDsToTrigger[i]).InitializeGoal();
            }
            SetComplete();
        }
    }
}