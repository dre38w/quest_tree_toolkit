using Service.Framework.GoalManagement;
using Service.Framework.Goals;
using System.Collections.Generic;
using UnityEngine;

public class QuestLogUI : MonoBehaviour
{
    [SerializeField]
    private Transform questContent;
    [SerializeField]
    private QuestEntryUI questEntryPrefab;

    private Dictionary<GoalID, QuestEntryUI> activeQuests = new Dictionary<GoalID, QuestEntryUI>();
    private Dictionary<GoalID, int> objectiveCounts = new Dictionary<GoalID, int>();

    private GoalTrackerDatabase database;

    private void Start()
    {
        database = GoalManager.Instance.GoalTracker;
        database.OnObjectivesChanged.AddListener(OnObjectivesUpdated);
    }

    public void OnObjectivesUpdated(GoalID goal)
    {
        var objectives = database.GetObjectives(goal);
        if (objectives == null)
        {
            return;
        }

        if (!activeQuests.ContainsKey(goal))
        {
            var questUI = Instantiate(questEntryPrefab, questContent);
            questUI.Initialize(goal);

            activeQuests.Add(goal, questUI);
            objectiveCounts[goal] = 0;
        }

        var questEntry = activeQuests[goal];
        int currentCount = objectiveCounts[goal];

        for (int i = currentCount; i < objectives.Count; i++)
        {
            questEntry.AddObjective(objectives[i]);
        }

        questEntry.RefreshObjectives(objectives);
        objectiveCounts[goal] = objectives.Count;
    }
}