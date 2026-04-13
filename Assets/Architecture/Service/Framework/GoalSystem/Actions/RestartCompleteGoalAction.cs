/*
 * Description: Restarts a goal that has already completed
 */
using Service.Core;
using Service.Framework.GoalManagement;
using UnityEngine;

namespace Service.Framework.Goals
{
    [Submenu("Goal Management/Restart Completed Goal")]
    public class RestartCompleteGoalAction : ObjectiveAction
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
            if (GoalManager.Instance.GetGoal(goalToCheck).IsComplete())
            {
                for (int i = 0; i < goalsToRestart.Length; i++)
                {
                    GoalManager.Instance.GetGoal(goalsToRestart[i]).ReinitializeGoal();
                }
            }            
        }
    }
}