/*
 * Description: Database that holds references to the objectives, goals, and quests.
 *          Can be accessed to use for quest log, future quest can access past quest data, etc.
 */
using System.Collections.Generic;
using UnityEngine.Events;

namespace Service.Framework.Goals
{
    public class GoalTrackerDatabase
    {
        public UnityEvent<QuestID> OnObjectivesChanged = new UnityEvent<QuestID>();

        private Dictionary<QuestID, List<ObjectiveData>> questObjectives = new Dictionary<QuestID, List<ObjectiveData>>();
        private Dictionary<QuestID, bool> questCompletion = new Dictionary<QuestID, bool>();

        /// <summary>
        /// Adds a new objective
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objectiveText">The text we want to access for more front end gameplay</param>
        public ObjectiveData AddObjective(QuestID id, string objectiveText)
        {
            if (!questObjectives.ContainsKey(id))
            {
                questObjectives[id] = new List<ObjectiveData>();
            }

            ObjectiveData data = new ObjectiveData(objectiveText);           
            questObjectives[id].Add(data);

            OnObjectivesChanged.Invoke(id);

            return data;
        }

        /// <summary>
        /// Removes a specific objective from the database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objectiveIndex"></param>
        public void RemoveObjective(QuestID id, int objectiveIndex)
        {
            if (questObjectives.ContainsKey(id))
            {
                questObjectives[id].RemoveAt(objectiveIndex);
            }
        }

        /// <summary>
        /// Marks a specific objective complete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objectiveIndex"></param>
        public void MarkObjectiveComplete(QuestID id, string objectiveID)
        {
            if (questObjectives.ContainsKey(id))
            {
                List<ObjectiveData> dataList = questObjectives[id];

                for (int i = 0; i < dataList.Count; i++)
                {
                    if (dataList[i].ID == objectiveID)
                    {
                        dataList[i].IsComplete = true;
                        OnObjectivesChanged.Invoke(id);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Completes the previously added objective.
        /// Useful for linear progression
        /// </summary>
        /// <param name="id"></param>
        public void CompleteLatestObjective(QuestID id)
        {
            if (questObjectives.ContainsKey(id))
            {
                List<ObjectiveData> objectives = questObjectives[id];

                if (objectives.Count > 0)
                {
                    //complete the previous index
                    objectives[objectives.Count - 1].IsComplete = true;
                    OnObjectivesChanged.Invoke(id);
                }
            }
        }

        /// <summary>
        /// Get a reference to a specific objective entry.
        /// Useful for 'recalling' a past event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objectiveIndex"></param>
        /// <returns>Returns a string, displaying what the front end user will see</returns>
        public string GetObjectiveEntry(QuestID id, int objectiveIndex)
        {
            if (questObjectives.ContainsKey(id))
            {
                return questObjectives[id][objectiveIndex].ObjectiveText;
            }
            return string.Empty;
        }

        /// <summary>
        /// Get all the objectives currently added
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<ObjectiveData> GetObjectives(QuestID id)
        {
            if (questObjectives.TryGetValue(id, out List<ObjectiveData> objectives))
            {
                return objectives;
            }
            return null;
        }

        /// <summary>
        /// Completes a quest
        /// </summary>
        /// <param name="id">The quest to complete</param>
        public void CompleteQuest(QuestID id)
        {
            questCompletion[id] = true;
            OnObjectivesChanged.Invoke(id);
        }

        /// <summary>
        /// Returns true when the quest is complete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsQuestComplete(QuestID id)
        {
            return questCompletion.ContainsKey(id) && questCompletion[id];
        }
    }
}