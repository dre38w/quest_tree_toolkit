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
        //[SerializeField]
        //private ObjectiveTrackerUI objectivePrefab;
        [SerializeField]
        private TrackedQuestEntryUI trackedQuestPrefab;

        [SerializeField]
        private ApplyLogEntryEffects entryEffects;

        [SerializeField]
        private Transform objectiveTrackerParent;

        //[SerializeField]
        //private TMP_Text trackedQuestText;

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

        private Dictionary<QuestID, TrackedQuestEntryUI> currentTrackedQuest = new Dictionary<QuestID, TrackedQuestEntryUI>();
        private Dictionary<QuestID, Dictionary<string, ObjectiveTrackerUI>> currentTrackedObjectives = new Dictionary<QuestID, Dictionary<string, ObjectiveTrackerUI>>();
        private Dictionary<QuestID, QuestEntryUI> activeQuests = new Dictionary<QuestID, QuestEntryUI>();
        private Dictionary<QuestID, Dictionary<string, ObjectiveEntryUI>> activeObjectives = new Dictionary<QuestID, Dictionary<string, ObjectiveEntryUI>>();

        private GoalTrackerDatabase database;

        private void Start()
        {
            database = GoalManager.Instance.GoalTracker;
            database.OnObjectivesChanged.AddListener(OnObjectivesUpdated);
            entryEffects.OnTrackedQuestHidden.AddListener(TrackedQuestHidden);
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
            }

            QuestEntryUI questEntry = activeQuests[id];

            foreach (ObjectiveData objective in objectives)
            {
                if (!activeObjectives[id].ContainsKey(objective.ID))
                {
                    ObjectiveEntryUI objectiveUI = questEntry.AddObjective(objective);
                    activeObjectives[id].Add(objective.ID, objectiveUI);
                }
                RefreshObjective(id, objective);
                RefreshTrackedObjectives(id, objective);
            }
            RefreshQuestVisual(id, GoalManager.Instance.GoalTracker.GetQuest(id));
            RefreshTrackedQuestVisual(id, GoalManager.Instance.GoalTracker.GetQuest(id));
            //refresh the objective entries in the event any of them just completed
            //RefreshTrackedObjectives(id, objective);
        }

        private void RefreshObjective(QuestID questID, ObjectiveData objective)
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
                objectiveUI.OnObjectiveFailed.Invoke();
                entryEffects.ApplyObjectiveFailed(objectiveUI, objective, hideCompletedObjectives);
            }
            else if (objective.IsComplete)
            {
                objectiveUI.OnObjectiveCompleted.Invoke();
                entryEffects.ApplyObjectiveComplete(objectiveUI, objective, hideCompletedObjectives);
            }
            else
            {
                entryEffects.ApplyObjectiveNormal(objectiveUI, objective);
            }
        }

        private void RefreshQuestVisual(QuestID id, QuestData data)
        {
            if (!activeQuests.TryGetValue(id, out QuestEntryUI questEntry))
            {
                return;
            }
            if (database.IsQuestComplete(id))
            {
                questEntry.OnQuestCompleted.Invoke();
                entryEffects.ApplyQuestComplete(questEntry, id, hideCompletedQuests);
            }
            else if (data.IsFailed)
            {
                questEntry.OnQuestFailed.Invoke();
                entryEffects.ApplyQuestFailed(questEntry, data, hideCompletedQuests);
            }
            else
            {
                entryEffects.ApplyQuestNormal(questEntry, id);
            }
        }

        public void SetTrackedQuest(QuestID id)
        {
            //ignore if we are already tracking
            if (trackedQuest == id)
            {
                return;
            }
            //clear old quest list
            ClearTrackedQuest();

            List<ObjectiveData> objectives = database.GetObjectives(id);
            if (objectives == null)
            {
                return;
            }
            //add a new quest entry if one has not yet been added to the log
            if (!currentTrackedQuest.ContainsKey(id))
            {
                TrackedQuestEntryUI questUI = Instantiate(trackedQuestPrefab, objectiveTrackerParent);
                questUI.Initialize(id);

                currentTrackedQuest.Add(id, questUI);
                currentTrackedObjectives[id] = new Dictionary<string, ObjectiveTrackerUI>();
            }

            TrackedQuestEntryUI questEntry = currentTrackedQuest[id];

            foreach (ObjectiveData objective in objectives)
            {
                if (!currentTrackedObjectives[id].ContainsKey(objective.ID))
                {
                    ObjectiveTrackerUI objectiveUI = questEntry.AddObjective(objective);
                    currentTrackedObjectives[id].Add(objective.ID, objectiveUI);
                }
                RefreshTrackedObjectives(id, objective);
            }
            RefreshTrackedQuestVisual(id, GoalManager.Instance.GoalTracker.GetQuest(id));

            //get the new quest we are trying to track
            trackedQuest = id;
            //trackedQuestText.text = id.questName;

            //List<ObjectiveData> data = database.GetObjectives(id);
            //if (data == null)
            //{
            //    return;
            //}
            //RefreshTrackedObjectives(id, objectives);
        }

        private void ClearTrackedQuest()
        {
            foreach (TrackedQuestEntryUI tracker in currentTrackedQuest.Values)
            {
                if (tracker != null)
                {
                    Destroy(tracker.gameObject);
                }
            }
            currentTrackedObjectives.Clear();
            currentTrackedQuest.Clear();
            //TODO: this will be instantiated
            //trackedQuestText.text = string.Empty;
        }

        /// <summary>
        /// TODO: rename to RefreshTrackedQuest
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        private void RefreshTrackedQuestVisual(QuestID id, QuestData data)
        {
            if (!currentTrackedQuest.TryGetValue(id, out TrackedQuestEntryUI questEntry))
            {
                return;
            }
            if (data.IsComplete)
            {
                questEntry.OnTrackedQuestCompleted.Invoke();
                entryEffects.ApplyTrackedQuestComplete(questEntry, id);
            }
            else if (data.IsFailed)
            {
                questEntry.OnTrackedQuestFailed.Invoke();
                entryEffects.ApplyTrackedQuestFailed(questEntry, data);
            }
            else
            {
                entryEffects.ApplyTrackedQuestNormal(questEntry, id);
            }
        }

        private void RefreshTrackedObjectives(QuestID questID, ObjectiveData objective)
        {
            if (!currentTrackedObjectives.TryGetValue(questID, out Dictionary<string, ObjectiveTrackerUI> objectiveEntries))
            {
                return;
            }
            if (!objectiveEntries.TryGetValue(objective.ID, out ObjectiveTrackerUI objectiveUI))
            {
                return;
            }

            if (objective.IsFailed)
            {
                objectiveUI.OnTrackedObjectiveFailed.Invoke();
                entryEffects.ApplyTrackedObjectiveFailed(objectiveUI, objective);
            }
            else if (objective.IsComplete)
            {
                objectiveUI.OnTrackedObjectiveCompleted.Invoke();
                entryEffects.ApplyTrackedObjectiveComplete(objectiveUI, objective);
            }
            else
            {
                entryEffects.ApplyTrackedObjectiveNormal(objectiveUI, objective);
            }
            //if (trackedQuest != questID)
            //{
            //    return;
            //}

            //foreach (ObjectiveData objective in objectives)
            //{
            //    if (!currentTrackedObjectives.TryGetValue(objective.ID, out ObjectiveTrackerUI objectiveTrackerUI))
            //    {
            //        objectiveTrackerUI = Instantiate(objectivePrefab, objectiveTrackerParent.transform);
            //        //initialize data
            //        objectiveTrackerUI.Initialize(objective);

            //        currentTrackedObjectives.Add(objective.ID, objectiveTrackerUI);

            //    }

            //    //if (!currentTrackedObjectives.ContainsKey(objective.ID))
            //    //{
            //    //    currentObjectiveTracked = Instantiate(objectivePrefab, objectiveTrackerParent.transform);
            //    //    //currentObjectiveTracked = trackerUI;
            //    //    currentObjectiveTracked.Initialize(objective);

            //    //    currentTrackedObjectives.Add(objective.ID, currentObjectiveTracked);
            //    //}
            //    RefreshTrackedObjectiveVisuals(objective);
            //    //currentObjectiveTracked.InitializeVisuals(objective);
            //}
        }

        public void TrackedQuestHidden()
        {
            ClearTrackedQuest();
        }

        //private void RefreshTrackedObjectiveVisuals(ObjectiveData objective)
        //{
        //    if (!currentTrackedObjectives.TryGetValue(objective.ID, out ObjectiveTrackerUI trackerUI))
        //    {
        //        return;
        //    }
        //    if (objective.IsFailed)
        //    {
        //        entryEffects.ApplyTrackedObjectiveFailed(trackerUI, objective);
        //    }
        //    else if (objective.IsComplete)
        //    {
        //        entryEffects.ApplyTrackedObjectiveComplete(trackerUI, objective);
        //    }
        //    else
        //    {
        //        entryEffects.ApplyTrackedObjectiveNormal(trackerUI, objective);
        //    }
        //}

        private void OnDestroy()
        {
            database.OnObjectivesChanged.RemoveListener(OnObjectivesUpdated);
            entryEffects.OnTrackedQuestHidden.RemoveListener(TrackedQuestHidden);
        }
    }
}