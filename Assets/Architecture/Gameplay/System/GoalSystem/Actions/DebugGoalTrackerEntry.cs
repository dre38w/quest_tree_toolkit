using Service.Framework.GoalManagement;
using Service.Framework.Goals;
using UnityEngine;

public class DebugGoalTrackerEntry : ObjectiveAction
{
    public override void InitializeAction()
    {
        var objectives = GoalManager.Instance.GoalTracker.GetObjectives(ActionGoalID);

        for (int i = 0; i < objectives.Count; i++)
        {
            Debug.Log($"Updated entry: {GoalManager.Instance.GoalTracker.GetObjectiveEntry(ActionGoalID, i)}");
        }
        SetComplete();
    }
}
