/*
 * Description: Database that holds references to the objectives, goals, and quests.
 *          Can be accessed to use for quest log, future quest can access past quest data, etc.
 */
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace Service.Framework.Goals
{
    public class GoalTrackerDatabase
    {
        public UnityEvent<QuestID> OnObjectivesChanged = new UnityEvent<QuestID>();

        private Dictionary<QuestID, List<QuestData>> quests = new Dictionary<QuestID, List<QuestData>>();
        private Dictionary<QuestID, List<ObjectiveData>> questObjectives = new Dictionary<QuestID, List<ObjectiveData>>();
        private Dictionary<QuestID, bool> questCompletion = new Dictionary<QuestID, bool>();

        /// <summary>
        /// Add a quest
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public QuestData AddQuest(QuestID id)
        {
            //create a new one if not yet created
            if (!quests.ContainsKey(id))
            {
                quests[id] = new List<QuestData>();
            }
            QuestData data = new QuestData(id);

            //if we find this already exists, early exit
            if (quests[id].Contains(quests[id].Find(d => d.ID == data.ID)))
            {
                return null;
            }
            //add the new quest
            quests[id].Add(data);
            return data;
        }

        /// <summary>
        /// Adds a new objective
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objectiveText">The text we want to access for more front end gameplay</param>
        public ObjectiveData AddObjective(QuestID id, string objectiveText, bool isSubObjective, string parentID = null)
        {            
            if (!questObjectives.ContainsKey(id))
            {
                questObjectives[id] = new List<ObjectiveData>();
            }

            ObjectiveData data = new ObjectiveData(objectiveText, isSubObjective, parentID);
            
            questObjectives[id].Add(data);

            if (isSubObjective && parentID != null)
            {
                ObjectiveData baseObjective = questObjectives[id].Find(b => b.ID == parentID);

                if (baseObjective != null)
                {
                    baseObjective.SubObjectivesIDs.Add(data.ID);
                }
            }
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

        public void MarkObjectiveFailed(QuestID id, string objectiveID, bool failQuest, bool markComplete = false)
        {
            if (!questObjectives.ContainsKey(id))
            {
                return;
            }

            List<ObjectiveData> dataList = questObjectives[id];
            ObjectiveData target = dataList.Find(o => o.ID == objectiveID);

            if (target == null)
            {
                return;
            }
            target.IsFailed = true;

            //mark complete if this fail state is meant to permanently fail the objective 
            if (markComplete)
            {
                target.IsComplete = true;
            }

            if (failQuest)
            {
                FailQuest(id);
            }
            OnObjectivesChanged.Invoke(id);
        }

        /// <summary>
        /// Force restart an objective
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objectiveID"></param>
        public void RestartObjective(QuestID id, string objectiveID)
        {
            if (!questObjectives.ContainsKey(id))
            {
                return;
            }

            List<ObjectiveData> dataList = questObjectives[id];
            ObjectiveData target = dataList.Find(o => o.ID == objectiveID);

            if (target == null)
            {
                return;
            }
            target.IsFailed = false;
            target.IsComplete = false;

            OnObjectivesChanged.Invoke(id);
        }

        /// <summary>
        /// Marks a specific objective complete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objectiveIndex"></param>
        public void MarkObjectiveComplete(QuestID id, string objectiveID)
        {
            if (!questObjectives.ContainsKey(id))
            {
                return;
            }
            List<ObjectiveData> dataList = questObjectives[id];
            ObjectiveData target = dataList.Find(o => o.ID == objectiveID);

            if (target == null)
            {
                return;
            }
            target.IsComplete = true;
            target.IsFailed = false;

            if (target.IsSubObjective && !string.IsNullOrEmpty(target.ParentObjectiveID))
            {
                ObjectiveData parentObjective = dataList.Find(p => p.ID == target.ParentObjectiveID);

                //if all subobjectives are complete, complete the parent
                if (parentObjective.SubObjectivesIDs.All(subID => dataList.Find(o => o.ID == subID).IsComplete == true))
                {
                    parentObjective.IsComplete = true;
                }
            }
            OnObjectivesChanged.Invoke(id);
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
        /// Get the objective by its index
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objectiveIndex"></param>
        /// <returns></returns>
        public ObjectiveData GetObjective(QuestID id, int objectiveIndex)
        {
            if (!questObjectives.ContainsKey(id))
            {
                return null;
            }
            List<ObjectiveData> objectives = questObjectives[id];

            if (objectiveIndex < 0 || objectiveIndex >= objectives.Count)
            {
                return null;
            }
            return objectives[objectiveIndex];
        }

        /// <summary>
        /// Get the objective by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objectiveID"></param>
        /// <returns></returns>
        public ObjectiveData GetObjective(QuestID id, string objectiveID)
        {
            if (string.IsNullOrEmpty(objectiveID))
            {
                return null;
            }
            if (!questObjectives.TryGetValue(id, out List<ObjectiveData> objectives))
            {
                return null;
            }
            return objectives.Find(o => o.ID == objectiveID);
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

        public QuestData GetQuest(QuestID id)
        {
            return quests[id].Find(q => q.ID == id);
        }

        public void FailQuest(QuestID id)
        {
            GetQuest(id).IsFailed = true;
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