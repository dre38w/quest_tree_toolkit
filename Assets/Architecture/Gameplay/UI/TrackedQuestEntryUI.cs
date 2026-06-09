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
            OnTrackedQuestAdded.Invoke();
            questTitle.text = id.questName;
        }

        public ObjectiveTrackerUI AddObjective(ObjectiveData data)
        {
            ObjectiveTrackerUI newObjective = Instantiate(objectivePrefab, objectivesContent);
            newObjective.Initialize(data);
            return newObjective;
        }
    }
}