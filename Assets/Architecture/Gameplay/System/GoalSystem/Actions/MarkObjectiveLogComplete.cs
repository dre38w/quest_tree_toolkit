using Service.Framework.GoalManagement;
using Service.Framework.Goals;
using UnityEngine;

namespace Gameplay.System.Actions
{
    public class MarkObjectiveLogComplete : ObjectiveAction
    {
        public override void InitializeAction()
        {
            GoalManager.Instance.GoalTracker.CompleteLatestObjective(ActionGoalID);
            SetComplete();
        }
    }
}