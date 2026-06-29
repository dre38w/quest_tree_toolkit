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

        /// <summary>
        /// Dictionaries that hold the backend data
        /// </summary>
        private Dictionary<QuestID, List<QuestData>> quests = new Dictionary<QuestID, List<QuestData>>();
        private Dictionary<GoalID, List<GoalData>> goals = new Dictionary<GoalID, List<GoalData>>();
        private Dictionary<QuestID, List<ObjectiveData>> questObjectives = new Dictionary<QuestID, List<ObjectiveData>>();
        private Dictionary<QuestID, Dictionary<ObjectiveID, ObjectiveData>> recordedObjectives = new Dictionary<QuestID, Dictionary<ObjectiveID, ObjectiveData>>();
        private Dictionary<QuestID, bool> questCompletion = new Dictionary<QuestID, bool>();

        /// <summary>
        /// Add a quest
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public QuestData AddQuest(QuestID id)
        {
            //if it doesn't exist, create a list to place it in
            if (!quests.TryGetValue(id, out List<QuestData> questList))
            {
                questList = new List<QuestData>();
                quests[id] = questList;
            }

            //check to see if we already added this quest
            if (questList.Any(q => q.ID == id))
            {
                return null;
            }

            //no quest was found, so create one
            QuestData data = new QuestData(id);
            questList.Add(data);

            return data;
        }

        /// <summary>
        /// Add a goal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GoalData AddGoal(GoalID id)
        {
            if (!goals.TryGetValue(id, out List<GoalData> goalList))
            {
                goalList = new List<GoalData>();
                goals[id] = goalList;
            }

            if (goalList.Any(g => g.ID == id))
            {
                return null;
            }
            //no matching goal was found, create one
            GoalData data = new GoalData(id);
            goalList.Add(data);

            return data;
        }

        /// <summary>
        /// Adds a new objective
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objectiveText">The text we want to access for more front end gameplay</param>
        public ObjectiveData AddObjective(QuestID id, string objectiveText, bool isSubObjective, string parentID = null, ObjectiveID objectiveID = null)
        {
            //no list of quest objectives was found, so create one
            if (!questObjectives.ContainsKey(id))
            {
                questObjectives[id] = new List<ObjectiveData>();
            }

            //create a new data instance
            ObjectiveData data = new ObjectiveData(objectiveText, isSubObjective, parentID, objectiveID);

            //add to the new one to the dictionary
            questObjectives[id].Add(data);

            //handle sub objectives
            if (isSubObjective && parentID != null)
            {
                //find the objective this is a child of
                ObjectiveData baseObjective = questObjectives[id].Find(b => b.ID == parentID);

                //add this objective's id to the parent's list of sub objectives to properly track
                if (baseObjective != null)
                {
                    baseObjective.SubObjectivesIDs.Add(data.ID);
                }
            }
            OnObjectivesChanged.Invoke(id);

            return data;
        }

        /// <summary>
        /// Adds a recorded objective 
        /// </summary>
        /// <param name="questID"></param>
        /// <param name="objectiveID"></param>
        /// <returns></returns>
        public ObjectiveData AddRecordedObjective(QuestID questID, ObjectiveID objectiveID)
        {
            if (objectiveID == null)
            {
                return null;
            }

            if (!recordedObjectives.TryGetValue(questID, out Dictionary<ObjectiveID, ObjectiveData> objectiveMap))
            {
                objectiveMap = new Dictionary<ObjectiveID, ObjectiveData>();
                recordedObjectives[questID] = objectiveMap;
            }
            if (objectiveMap.TryGetValue(objectiveID, out ObjectiveData existingData))
            {
                return existingData;
            }

            ObjectiveData data = new ObjectiveData(textEntry: string.Empty, isSubObjective: false, parentID: null, objectiveID: objectiveID);
            objectiveMap.Add(objectiveID, data);

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
        /// Used to fail an objective that is displayed via UI
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objectiveID"></param>
        /// <param name="failQuest"></param>
        /// <param name="markComplete"></param>
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
        /// Used to set the fail state of a backend recorded objective action
        /// </summary>
        /// <param name="questID"></param>
        /// <param name="objectiveID"></param>
        /// <param name="failQuest"></param>
        /// <param name="markComplete"></param>
        public void MarkObjectiveFailed(QuestID questID, ObjectiveID objectiveID, bool failQuest, bool markComplete = false)
        {
            ObjectiveData target = GetObjective(questID, objectiveID);

            if (target == null)
            {
                return;
            }
            target.IsFailed = true;

            if (markComplete)
            {
                target.IsComplete = true;
            }

            if (failQuest)
            {
                FailQuest(questID);
            }
        }

        /// <summary>
        /// Fail the previously added objective
        /// </summary>
        /// <param name="id"></param>
        public void FailLatestObjective(QuestID id)
        {
            if (questObjectives.ContainsKey(id))
            {
                List<ObjectiveData> objectives = questObjectives[id];

                if (objectives.Count > 0)
                {
                    objectives[objectives.Count - 1].IsFailed = true;
                    OnObjectivesChanged.Invoke(id);
                }
            }
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
        public void MarkObjectiveComplete(QuestID id, string objectiveID, bool subObjectivesComplete)
        {
            if (!questObjectives.ContainsKey(id))
            {
                return;
            }
            //find the target objective
            List<ObjectiveData> dataList = questObjectives[id];
            ObjectiveData target = dataList.Find(o => o.ID == objectiveID);

            //early out if there is no target to handle
            if (target == null)
            {
                return;
            }
            target.IsComplete = true;
            target.IsFailed = false;

            //handle auto completing a parent objective
            if (target.IsSubObjective && !string.IsNullOrEmpty(target.ParentObjectiveID) && subObjectivesComplete)
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
        /// Overload that handles completing a recorded objective
        /// </summary>
        /// <param name="questID"></param>
        /// <param name="objectiveID"></param>
        public void MarkObjectiveComplete(QuestID questID, ObjectiveID objectiveID)
        {
            ObjectiveData target = GetObjective(questID, objectiveID);

            if (target == null)
            {
                return;
            }

            target.IsComplete = true;
            target.IsFailed = false;

            //OnObjectivesChanged.Invoke(questID);
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
        /// Get a reference to a specific objective text entry.
        /// Useful for 'recalling' a past event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objectiveIndex"></param>
        /// <returns>Returns a string, displaying what the front end user will see</returns>
        public string GetObjectiveEntry(QuestID id, int objectiveIndex)
        {
            if (questObjectives.ContainsKey(id))
            {
                string objectiveText = questObjectives[id][objectiveIndex].ObjectiveText;

                if (!string.IsNullOrEmpty(objectiveText))
                {
                    return objectiveText;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Get a reference to a specific recorded objective.
        /// </summary>
        /// <param name="questID"></param>
        /// <param name="objectiveID"></param>
        /// <returns></returns>
        public ObjectiveData GetObjective(QuestID questID, ObjectiveID objectiveID)
        {
            if (objectiveID == null)
            {
                return null;
            }

            //if the recorded objective map doesn't exist, early out
            if (!recordedObjectives.TryGetValue(questID, out Dictionary<ObjectiveID, ObjectiveData> objectiveMap))
            {
                return null;
            }
            //try to get the objective's data within that map
            objectiveMap.TryGetValue(objectiveID, out ObjectiveData data);
            return data;
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
        /// Get the objective by its GUID
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

        /// <summary>
        /// Find a specific quest
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public QuestData GetQuest(QuestID id)
        {
            return quests[id].Find(q => q.ID == id);
        }

        /// <summary>
        /// Find a specific goal
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GoalData GetGoal(GoalID id)
        {
            return goals[id].Find(g => g.ID == id);
        }

        /// <summary>
        /// Set a quest as failed
        /// </summary>
        /// <param name="id"></param>
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