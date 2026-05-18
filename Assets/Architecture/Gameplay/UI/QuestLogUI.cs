/*
 * Description: Manages the quest log UI
 */
using Service.Framework;
using Service.Framework.GoalManagement;
using Service.Framework.Goals;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gameplay.UI
{
    public class QuestLogUI : MonoBehaviour
    {
        [SerializeField]
        private Transform questContent;
        [SerializeField]
        private QuestEntryUI questEntryPrefab;
        [SerializeField]
        private ObjectiveTrackerUI objectivePrefab;

        [SerializeField]
        private LogEntryEffects entryEffects;

        [SerializeField]
        private GameObject objectiveTrackerParent;

        [SerializeField]
        private TMP_Text trackedQuestText;

        [Tooltip("Should the log hide the completed objectives or visually show them complete?")]
        [SerializeField]
        private bool hideCompletedObjectives;
        public bool HideCompletedObjectives
        {
            get { return hideCompletedObjectives; }
            set { hideCompletedObjectives = value; }
        }
        [Tooltip("Should the log hide the completed quests or visually show them complete?")]
        [SerializeField]
        private bool hideCompletedQuests;
        public bool HideCompletedQuests
        {
            get { return hideCompletedQuests; }
            set { hideCompletedQuests = value; }
        }

        private QuestID trackedQuest;
        //private List<ObjectiveTrackerUI> currentTrackedObjectives = new List<ObjectiveTrackerUI>();

        private Dictionary<string, ObjectiveTrackerUI> currentTrackedObjectives = new Dictionary<string, ObjectiveTrackerUI>();
        private Dictionary<QuestID, QuestEntryUI> activeQuests = new Dictionary<QuestID, QuestEntryUI>();
        private Dictionary<QuestID, Dictionary<string, ObjectiveEntryUI>> activeObjectives = new Dictionary<QuestID, Dictionary<string, ObjectiveEntryUI>>();
        //private Dictionary<QuestID, int> objectiveCounts = new Dictionary<QuestID, int>();

        private GoalTrackerDatabase database;

        private void Start()
        {
            database = GoalManager.Instance.GoalTracker;
            database.OnObjectivesChanged.AddListener(OnObjectivesUpdated);
        }

        /// <summary>
        /// Handles managing the objectives and quest UI entries
        /// </summary>
        /// <param name="id">The quest we are working with</param>
        public void OnObjectivesUpdated(QuestID id)
        {
            //pull the objectives each time they update to work with the latest list
            List<ObjectiveData> objectives = database.GetObjectives(id);
            if (objectives == null)
            {
                return;
            }

            //add a new quest entry if one has not yet been added to the log
            if (!activeQuests.ContainsKey(id))
            {
                QuestEntryUI questUI = Instantiate(questEntryPrefab, questContent);
                questUI.Initialize(id);

                activeQuests.Add(id, questUI);
                activeObjectives[id] = new Dictionary<string, ObjectiveEntryUI>();
               // objectiveCounts[id] = 0;

            }

            QuestEntryUI questEntry = activeQuests[id];

            foreach (ObjectiveData objective in objectives)
            {
                if (!activeObjectives[id].ContainsKey(objective.ID))
                {
                    ObjectiveEntryUI objectiveUI = questEntry.AddObjective(objective);
                    activeObjectives[id].Add(objective.ID, objectiveUI);
                }
                RefreshObjectiveVisual(id, objective);
            }
            RefreshQuestVisual(id);

            //int currentCount = objectiveCounts[id];

            ////add a new objective
            //for (int i = currentCount; i < objectives.Count; i++)
            //{
            //    questEntry.AddObjective(objectives[i]);
            //}

            ////refresh the objective entries in the event any of them just completed
            //questEntry.RefreshObjectives(objectives, hideCompletedObjectives);
            //questEntry.RefreshQuestState(database.IsQuestComplete(id), hideCompletedQuests);

            RefreshTrackedObjectives(id, objectives);
            //if (currentTrackedObjectives.Count > 0)
            //{
            //    for (int i = 0; i < currentTrackedObjectives.Count; i++)
            //    {
            //        currentTrackedObjectives[i].RefreshObjectives(id);
            //    }
            //}
            //objectiveCounts[id] = objectives.Count;
        }

        private void RefreshObjectiveVisual(QuestID questID, ObjectiveData objective)
        {
            if (!activeObjectives.TryGetValue(questID, out Dictionary<string, ObjectiveEntryUI> objectiveEntries))
            {
                return;
            }
            if (!objectiveEntries.TryGetValue(objective.ID, out ObjectiveEntryUI objectiveUI))
            {
                return;
            }

            if (objective.IsFailed)
            {
                entryEffects.ApplyObjectiveFailed(objectiveUI, objective);
                //ApplyObjectiveFailedEffect(objectiveUI, objective);
                //return;
            }
            else if (objective.IsComplete)
            {
                entryEffects.ApplyObjectiveComplete(objectiveUI, objective, hideCompletedObjectives);
                //return;
            }
            else
            {
                entryEffects.ApplyObjectiveNormal(objectiveUI, objective);
            }
        }

        //private void ApplyObjectiveActiveEffect(ObjectiveEntryUI objectiveUI, ObjectiveData objective)
        //{
        //    objectiveUI.SetNormal(objective);
        //}

        //private void ApplyObjectiveCompleteEffect(ObjectiveEntryUI objectiveUI, ObjectiveData objective)
        //{
        //    if (hideCompletedObjectives)
        //    {
        //        objectiveUI.Hide();
        //    }
        //    else
        //    {
        //        objectiveUI.SetComplete();
        //    }
        //}

        //private void ApplyObjectiveFailedEffect(ObjectiveEntryUI objectiveUI, ObjectiveData objective)
        //{
        //    objectiveUI.SetFailed(objective);
        //}

        private void RefreshQuestVisual(QuestID id)
        {
            if (!activeQuests.TryGetValue(id, out QuestEntryUI questEntry))
            {
                return;
            }
            if (database.IsQuestComplete(id))
            {
                entryEffects.ApplyQuestComplete(questEntry, id, hideCompletedQuests);
            }
            else
            {
                entryEffects.ApplyQuestNormal(questEntry, id);
            }
        }

        public void SetTrackedQuest(QuestID questID)
        {
            if (trackedQuest == questID)
            {
                return;
            }
            //clear old quest list
            ClearTrackedObjectives();
            //for (int i = currentTrackedObjectives.Count - 1; i >= 0; i--)
            //{
            //    Destroy(currentTrackedObjectives[i].gameObject);
            //}
            //currentTrackedObjectives.Clear();
            //trackedQuestText.text = string.Empty;

            //get the new one
            trackedQuest = questID;
            trackedQuestText.text = questID.questName;

            List<ObjectiveData> data = database.GetObjectives(questID);
            if (data == null)
            {
                return;
            }
            RefreshTrackedObjectives(questID, data);
            //for (int i = 0; i < data.Count; i++)
            //{
            //    if (data[i].IsComplete)
            //    {
            //        continue;
            //    }
            //    UpdateActiveQuest(trackedQuest, data[i]);
            //}
        }

        private void ClearTrackedObjectives()
        {
            foreach (ObjectiveTrackerUI tracker in currentTrackedObjectives.Values)
            {
                if (tracker != null)
                {
                    Destroy(tracker.gameObject);
                }
            }
            currentTrackedObjectives.Clear();
            trackedQuestText.text = string.Empty;
        }

        //private void UpdateActiveQuest(QuestID questID, ObjectiveData data)
        //{
        //    //may not need this check
        //    if (trackedQuest != questID)
        //    {
        //        return;
        //    }
        //    if (database.IsQuestComplete(questID))
        //    {
        //        //objectiveData.Clear();

        //        //invoke quest complete here

        //        //clear everything

        //        return;
        //    }
        //    trackedQuestText.text = questID.questName;

        //    //if (currentTrackedObjectives.Count > 0)
        //    //{
        //    //    for (int i = currentTrackedObjectives.Count - 1; i >= 0; i--)
        //    //    {
        //    //        Destroy(currentTrackedObjectives[i].gameObject);
        //    //    }
        //    //    currentTrackedObjectives.Clear();
        //    //}

        //    if (data.IsComplete)
        //    {
        //        ObjectiveTrackerUI completedObjective = currentTrackedObjectives.Find(d => d.ObjectiveID == data.ID);
        //        Destroy(completedObjective.gameObject);
        //        currentTrackedObjectives.Remove(completedObjective);
        //        data = null;
        //        return;
        //    }

        //    ObjectiveTrackerUI newObjectiveText = Instantiate(objectivePrefab, objectiveTrackerParent.transform);
        //    newObjectiveText.GetComponent<TMP_Text>().text = data.ObjectiveText;
        //    newObjectiveText.Initialize(data);

        //    currentTrackedObjectives.Add(newObjectiveText);
        //}

        private void RefreshTrackedObjectives(QuestID questID, List<ObjectiveData> objectives)
        {
            if (trackedQuest != questID)
            {
                return;
            }

            foreach (ObjectiveData objective in objectives)
            {
                if (!currentTrackedObjectives.ContainsKey(objective.ID))
                {
                    ObjectiveTrackerUI trackerUI = Instantiate(objectivePrefab, objectiveTrackerParent.transform);
                    trackerUI.Initialize(objective);

                    currentTrackedObjectives.Add(objective.ID, trackerUI);
                }
                RefreshTrackedObjectiveVisuals(objective);
            }
        }

        private void RefreshTrackedObjectiveVisuals(ObjectiveData objective)
        {
            if (!currentTrackedObjectives.TryGetValue(objective.ID, out ObjectiveTrackerUI trackerUI))
            {
                return;
            }
            if (objective.IsFailed)
            {
                entryEffects.ApplyTrackedObjectiveFailed(trackerUI, objective);
            }
            else if (objective.IsComplete)
            {
                entryEffects.ApplyTrackedObjectiveComplete(trackerUI, objective, hideCompletedObjectives);
                //return;
            }
            else
            {
                entryEffects.ApplyTrackedObjectiveNormal(trackerUI, objective);
            }
        }
    }
}