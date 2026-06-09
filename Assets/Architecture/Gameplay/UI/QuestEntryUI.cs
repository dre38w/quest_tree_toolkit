
/*
 * Description: Handles displaying the quest title on the quest log UI
 */
using Gameplay.System;
using Service.Framework;
using Service.Framework.Goals;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.UI
{
    public class QuestEntryUI : MonoBehaviour
    {
        public UnityEvent OnQuestAdded = new UnityEvent();
        public UnityEvent OnQuestCompleted = new UnityEvent();
        public UnityEvent OnQuestFailed = new UnityEvent();

        [SerializeField]
        private TMP_Text questTitle;
        public TMP_Text QuestTitle => questTitle;
        [SerializeField]
        private Transform objectivesContent;
        [SerializeField]
        private ObjectiveEntryUI objectivePrefab;

        [Tooltip("The duration to wait between state changes. \n" +
            "Ex. Before playing a UI effect, activating/deactivating the UI element, etc.")]
        [SerializeField]
        private float stateChangeDelay = 0f;
        public float StateChangeDelay => stateChangeDelay;

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
        public ObjectiveEntryUI AddObjective(ObjectiveData data)
        {
            ObjectiveEntryUI newObjective = Instantiate(objectivePrefab, objectivesContent);
            newObjective.Initialize(data);
            return newObjective;
        }

        /// <summary>
        /// Called via external systems to set the active quest
        /// </summary>
        public void SetActiveQuest()
        {
            ReferenceRegistry.Instance.MainUI.GetComponent<QuestLogUI>().SetTrackedQuest(questID);
        }

        public void RefreshQuestState(bool isComplete, bool hideComplete)
        {
            if (isComplete)
            {
                OnQuestCompleted.Invoke();
            }
        }
    }
}