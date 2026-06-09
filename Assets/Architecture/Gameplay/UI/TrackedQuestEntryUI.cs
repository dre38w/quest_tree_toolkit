/*
 * Description: Handles displaying the quest title on the tracked quest UI
 */
using Service.Framework;
using Service.Framework.Goals;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.UI
{
    public class TrackedQuestEntryUI : MonoBehaviour
    {
        public UnityEvent OnTrackedQuestAdded = new UnityEvent();
        public UnityEvent OnTrackedQuestCompleted = new UnityEvent();
        public UnityEvent OnTrackedQuestFailed = new UnityEvent();

        [SerializeField]
        private TMP_Text questTitle;
        public TMP_Text QuestTitle => questTitle;
        [SerializeField]
        private Transform objectivesContent;
        [SerializeField]
        private ObjectiveTrackerUI objectivePrefab;

        [Tooltip("The duration to wait between state changes. \n" +
            "Ex. Before playing a UI effect, activating/deactivating the UI element, etc.")]
        [SerializeField]
        private float stateChangeDelay = 0f;
        public float StateChangeDelay => stateChangeDelay;

        public void Initialize(QuestID id)
        {
            //add the quest and set its name
            OnTrackedQuestAdded.Invoke();
            questTitle.text = id.questName;
        }

        /// <summary>
        /// Spawn the associated objectives
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ObjectiveTrackerUI AddObjective(ObjectiveData data)
        {
            ObjectiveTrackerUI newObjective = Instantiate(objectivePrefab, objectivesContent);
            //initialize data
            newObjective.Initialize(data);
            return newObjective;
        }
    }
}