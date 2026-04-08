using Service.Framework.Goals;
using System.Collections.Generic;
using UnityEngine.Events;

public class GoalTrackerDatabase
{
    public UnityEvent<GoalID> OnObjectivesChanged = new UnityEvent<GoalID>();

    public Dictionary<GoalID, List<ObjectiveData>> goalObjectives = new Dictionary<GoalID, List<ObjectiveData>>();

    public void AddObjective(GoalID goalId, string objectiveText)
    {
        if (!goalObjectives.ContainsKey(goalId))
        {
            goalObjectives[goalId] = new List<ObjectiveData>();
        }
        goalObjectives[goalId].Add(new ObjectiveData(objectiveText));

        OnObjectivesChanged.Invoke(goalId);
    }

    public void RemoveObjective(GoalID goalId, int objectiveIndex)
    {
        if (goalObjectives.ContainsKey(goalId))
        {
            goalObjectives[goalId].RemoveAt(objectiveIndex);
        }
    }

    public void MarkObjectiveComplete(GoalID goalId, int objectiveIndex)
    {
        if (goalObjectives.ContainsKey(goalId))
        {
            goalObjectives[goalId][objectiveIndex].IsComplete = true;
            OnObjectivesChanged.Invoke(goalId);
        }
    }

    public void CompleteLatestObjective(GoalID goalId)
    {
        if (goalObjectives.ContainsKey(goalId))
        {
            var objectives = goalObjectives[goalId];

            if (objectives.Count > 0)
            {
                objectives[objectives.Count - 1].IsComplete = true;
                OnObjectivesChanged.Invoke(goalId);
            }
        }
    }

    public string GetObjectiveEntry(GoalID goalId, int objectiveIndex)
    {
        if (goalObjectives.ContainsKey(goalId))
        {
            return goalObjectives[goalId][objectiveIndex].ObjectiveText;
        }
        return string.Empty;
    }

    public List<ObjectiveData> GetObjectives(GoalID goalId)
    {
        if (goalObjectives.TryGetValue(goalId, out List<ObjectiveData> objectives))
        {
            return objectives;
        }
        return null;
    }
}
