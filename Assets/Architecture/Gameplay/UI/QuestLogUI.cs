/*
 * Description: Manages the quest log UI
 */
using Service.Framework;
using Service.Framework.GoalManagement;
using Service.Framework.Goals;
using System.Collections.Generic;
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
        private TrackedQuestEntryUI trackedQuestPrefab;

        [SerializeField]
        private ApplyLogEntryEffects entryEffects;

        [SerializeField]
        private Transform objectiveTrackerParent;

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

            //manage the objectives
            foreach (ObjectiveData objective in objectives)
            {
                //add one if not yet created
                if (!activeObjectives[id].ContainsKey(objective.ID))
                {
                    ObjectiveEntryUI objectiveUI = questEntry.AddObjective(objective);
                    activeObjectives[id].Add(objective.ID, objectiveUI);
                }
                //refresh objective  visuals and any relavent data
                RefreshObjective(id, objective);
                RefreshTrackedObjectives(id, objective);
            }
            //handle refreshing the quests
            RefreshQuestVisual(id, GoalManager.Instance.GoalTracker.GetQuest(id));
            RefreshTrackedQuest(id, GoalManager.Instance.GoalTracker.GetQuest(id));
        }

        /// <summary>
        /// Refresh the objectives visuals and update data
        /// </summary>
        /// <param name="questID"></param>
        /// <param name="objective"></param>
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

            //handle failing
            if (objective.IsFailed)
            {
                objectiveUI.OnObjectiveFailed.Invoke();
                entryEffects.ApplyObjectiveFailed(objectiveUI, objective, hideCompletedObjectives);
            }
            //handling successful completion
            else if (objective.IsComplete)
            {
                objectiveUI.OnObjectiveCompleted.Invoke();
                entryEffects.ApplyObjectiveComplete(objectiveUI, objective, hideCompletedObjectives);
            }
            //handle normal active state
            else
            {
                entryEffects.ApplyObjectiveNormal(objectiveUI, objective);
            }
        }

        /// <summary>
        /// Refresh quest visuals and update data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        private void RefreshQuestVisual(QuestID id, QuestData data)
        {
            if (!activeQuests.TryGetValue(id, out QuestEntryUI questEntry))
            {
                return;
            }
            //handle successful completion
            if (database.IsQuestComplete(id))
            {
                questEntry.OnQuestCompleted.Invoke();
                entryEffects.ApplyQuestComplete(questEntry, id, hideCompletedQuests);
            }
            //handle failing
            else if (data.IsFailed)
            {
                questEntry.OnQuestFailed.Invoke();
                entryEffects.ApplyQuestFailed(questEntry, data, hideCompletedQuests);
            }
            //handle normal active quest state
            else
            {
                entryEffects.ApplyQuestNormal(questEntry, id);
            }
        }

        /// <summary>
        /// Set the quest to track
        /// </summary>
        /// <param name="id">The quest ID for the quest we want to track</param>
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
                //spawn the UI
                TrackedQuestEntryUI questUI = Instantiate(trackedQuestPrefab, objectiveTrackerParent);
                questUI.Initialize(id);

                //add the data
                currentTrackedQuest.Add(id, questUI);
                currentTrackedObjectives[id] = new Dictionary<string, ObjectiveTrackerUI>();
            }

            TrackedQuestEntryUI questEntry = currentTrackedQuest[id];

            foreach (ObjectiveData objective in objectives)
            {
                if (!currentTrackedObjectives[id].ContainsKey(objective.ID))
                {
                    //add the objective.  this will call an instantiate on the quest entry class
                    ObjectiveTrackerUI objectiveUI = questEntry.AddObjective(objective);
                    currentTrackedObjectives[id].Add(objective.ID, objectiveUI);
                }
                //refresh the tracked objectives to update visuals and data with the newly tracked objectives
                RefreshTrackedObjectives(id, objective);
            }
            //update the quest visuals and data
            RefreshTrackedQuest(id, GoalManager.Instance.GoalTracker.GetQuest(id));

            //get the new quest we are trying to track
            trackedQuest = id;
        }

        /// <summary>
        /// Clear the tracked quest and the objectives
        /// </summary>
        private void ClearTrackedQuest()
        {
            foreach (TrackedQuestEntryUI tracker in currentTrackedQuest.Values)
            {
                //visually clear the tracked quest
                if (tracker != null)
                {
                    Destroy(tracker.gameObject);
                }
            }
            //clear the tracked quest data
            currentTrackedObjectives.Clear();
            currentTrackedQuest.Clear();
        }

        /// <summary>
        /// Refreshes the tracked quest UI's visuals and updates any logic
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        private void RefreshTrackedQuest(QuestID id, QuestData data)
        {
            if (!currentTrackedQuest.TryGetValue(id, out TrackedQuestEntryUI questEntry))
            {
                return;
            }
            //handle successful completion
            if (data.IsComplete)
            {
                questEntry.OnTrackedQuestCompleted.Invoke();
                entryEffects.ApplyTrackedQuestComplete(questEntry, id);
            }
            //handle failure
            else if (data.IsFailed)
            {
                questEntry.OnTrackedQuestFailed.Invoke();
                entryEffects.ApplyTrackedQuestFailed(questEntry, data);
            }
            //handle normal active tracking
            else
            {
                entryEffects.ApplyTrackedQuestNormal(questEntry, id);
            }
        }

        /// <summary>
        /// Update the tracked objective visuals and data
        /// </summary>
        /// <param name="questID"></param>
        /// <param name="objective"></param>
        private void RefreshTrackedObjectives(QuestID questID, ObjectiveData objective)
        {
            //if this quest is not currently being tracked, don't do anything
            if (!currentTrackedObjectives.TryGetValue(questID, out Dictionary<string, ObjectiveTrackerUI> objectiveEntries) ||
                //safety check to make sure the objectives exist in the tracked quest
                !objectiveEntries.TryGetValue(objective.ID, out ObjectiveTrackerUI objectiveUI))
            {
                return;
            }
            //handle failed objectives
            if (objective.IsFailed)
            {
                objectiveUI.OnTrackedObjectiveFailed.Invoke();
                entryEffects.ApplyTrackedObjectiveFailed(objectiveUI, objective);
            }
            //handle successfully completed objectives
            else if (objective.IsComplete)
            {
                objectiveUI.OnTrackedObjectiveCompleted.Invoke();
                entryEffects.ApplyTrackedObjectiveComplete(objectiveUI, objective);
            }
            //handle normal active tracked objectives
            else
            {
                entryEffects.ApplyTrackedObjectiveNormal(objectiveUI, objective);
            }
        }

        /// <summary>
        /// Called when a tracked quest UI is hidden
        /// </summary>
        public void TrackedQuestHidden()
        {
            ClearTrackedQuest();
        }

        private void OnDestroy()
        {
            database.OnObjectivesChanged.RemoveListener(OnObjectivesUpdated);
            entryEffects.OnTrackedQuestHidden.RemoveListener(TrackedQuestHidden);
        }
    }
}