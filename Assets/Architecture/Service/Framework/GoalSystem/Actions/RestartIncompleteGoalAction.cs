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
        private GoalID[] goalsToRestart;

        public override void InitializeAction()
        {
            SetComplete();
                        
            if (!GoalManager.Instance.GetGoal(goalToCheck).IsComplete())
            {
                for (int i = 0; i < goalsToRestart.Length; i++)
                {
                    GoalManager.Instance.GetGoal(goalsToRestart[i]).ReinitializeGoal();
                }
            }
        }
    }
}