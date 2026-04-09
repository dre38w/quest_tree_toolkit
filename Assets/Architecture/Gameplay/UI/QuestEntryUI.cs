
/*
 * Description: Handles displaying the quest title on the quest log UI
 */
using Service.Framework;
using Service.Framework.Goals;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.UI
{
    public class QuestEntryUI : MonoBehaviour
    {
        public UnityEvent OnQuestAdded = new UnityEvent();
        public UnityEvent OnQuestCompleted = new UnityEvent();

        [SerializeField]
        private TMP_Text questTitle;
        [SerializeField]
        private Transform objectivesContent;
        [SerializeField]
        private ObjectiveEntryUI objectivePrefab;

        private List<ObjectiveEntryUI> spawnedObjectives = new List<ObjectiveEntryUI>();

        private QuestID questID;

        /// <summary>
        /// Can do any text enter effects here.
        /// </summary>
        /// <param name="id"></param>
        public void Initialize(QuestID id)
        {
            questID = id;

            OnQuestAdded.Invoke();
            questTitle.text = id.questName;
        }

        /// <summary>
        /// Spawn the objective entry
        /// </summary>
        /// <param name="data"></param>
        public void AddObjective(ObjectiveData data)
        {
            ObjectiveEntryUI newObjective = Instantiate(objectivePrefab, objectivesContent);
            newObjective.Initialize(data);

            spawnedObjectives.Add(newObjective);
        }

        public void RefreshObjectives(List<ObjectiveData> objectives, bool hideComplete)
        {
            for (int i = 0; i < spawnedObjectives.Count; i++)
            {
                //do some safety checks when completing the objective
                //since there are many ways to handle visually completing the objectives on the ui log
                if (hideComplete && objectives[i].IsComplete)
                {
                    //in the event we destroyed the object
                    //check null to make sure that's what we did 
                    //and then remove it
                    if (spawnedObjectives[i] == null)
                    {
                        spawnedObjectives.RemoveAt(i);
                        continue;
                    }
                }
                spawnedObjectives[i].RefreshEntry(objectives[i], hideComplete);
            }
        }

        public void RefreshQuestState(bool isComplete, bool hideComplete)
        {
            if (isComplete)
            {
                OnQuestCompleted.Invoke();

    ///****NOTE:  Below is some simple coded effects to show completing the quest****///

                if (hideComplete)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    questTitle.text = $"<s>{questID.questName}</s>";
                }
            }
        }
    }
}