using Service.Framework.Goals;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestEntryUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text questTitle;
    [SerializeField]
    private Transform objectivesContent;
    [SerializeField]
    private ObjectiveEntryUI objectivePrefab;

    private List<ObjectiveEntryUI> spawnedObjectives = new List<ObjectiveEntryUI>();

    public void Initialize(GoalID goal)
    {
        questTitle.text = goal.description;
    }

    public void AddObjective(ObjectiveData data)
    {
        var newObjective = Instantiate(objectivePrefab, objectivesContent);
        newObjective.SetText(data);

        spawnedObjectives.Add(newObjective);
    }

    public void RefreshObjectives(List<ObjectiveData> objectives)
    {
        for (int i = 0; i < spawnedObjectives.Count; i++)
        {
            spawnedObjectives[i].SetText(objectives[i]);
        }
    }
}