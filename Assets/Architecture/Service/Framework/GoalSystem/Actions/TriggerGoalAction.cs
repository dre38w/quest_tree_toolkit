using Service.Framework.Goals;
using UnityEngine;

namespace Service.Framework.Goals
{
    public class TriggerGoalAction : ObjectiveAction
    {
        [SerializeField]
        private GoalID goalIDToTrigger;

        public override void InitializeAction()
        {
            GoalManager.Instance.GetGoal(goalIDToTrigger).InitializeGoal();
            SetComplete();
        }
    }
}