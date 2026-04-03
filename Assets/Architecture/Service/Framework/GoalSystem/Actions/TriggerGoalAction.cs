/*
 * Description: Automatically triggers a specified goal to start
 */
using UnityEngine;

namespace Service.Framework.Goals
{
    public class TriggerGoalAction : ObjectiveAction
    {
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