/*
 * Description: Restarts a goal that has not yet completed
 */

using UnityEngine;

namespace Service.Framework.Goals
{
    public class RestartIncompleteGoalAction : ObjectiveAction
    {
        /// <summary>
        /// Use separate IDs in the event we want to restart a goal based on another goal's completion status
        /// </summary>
        [SerializeField]
        private GoalID goalToCheck;
        [SerializeField]
        private GoalID goalToRestart;

        public override void InitializeAction()
        {
            SetComplete();
            if (!GoalManager.Instance.GetGoal(goalToCheck).IsComplete())
            {
                GoalManager.Instance.GetGoal(goalToRestart).ReinitializeGoal();
            }
        }
    }
}