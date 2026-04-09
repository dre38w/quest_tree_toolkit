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

        private Dictionary<QuestID, QuestEntryUI> activeQuests = new Dictionary<QuestID, QuestEntryUI>();
        private Dictionary<QuestID, int> objectiveCounts = new Dictionary<QuestID, int>();

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
                objectiveCounts[id] = 0;
            }

            QuestEntryUI questEntry = activeQuests[id];
            int currentCount = objectiveCounts[id];

            //add a new objective
            for (int i = currentCount; i < objectives.Count; i++)
            {
                questEntry.AddObjective(objectives[i]);
            }

            //refresh the objective entries in the event any of them just completed
            questEntry.RefreshObjectives(objectives, hideCompletedObjectives);
            questEntry.RefreshQuestState(database.IsQuestComplete(id), hideCompletedQuests);

            objectiveCounts[id] = objectives.Count;
        }
    }
}