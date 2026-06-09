/*
 * Description: Handles displaying the tracked objective
 */
using Service.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.UI
{
    public class ObjectiveTrackerUI : MonoBehaviour
    {
        [Tooltip("Use this event to do effects when the objective is added.")]
        public UnityEvent OnTrackedObjectiveAdded = new UnityEvent();
        [Tooltip("Use this event to do effects when the objective is completed.")]
        public UnityEvent OnTrackedObjectiveCompleted = new UnityEvent();
        [Tooltip("Use this event to do effects when the objective fails.")]
        public UnityEvent OnTrackedObjectiveFailed = new UnityEvent();

        private string objectiveID;
        public string ObjectiveID
        {
            get { return objectiveID; }
            set { objectiveID = value; }
        }

        [SerializeField]
        private TMP_Text objectiveText;
        public TMP_Text ObjectiveText => objectiveText;

        [Tooltip("The duration to wait between state changes. \n" +
            "Ex. Before playing a UI effect, activating/deactivating the UI element, etc.")]
        [SerializeField]
        private float stateChangeDelay = 0f;
        public float StateChangeDelay => stateChangeDelay;

        /// <summary>
        /// Initialize data
        /// </summary>
        /// <param name="data"></param>
        public void Initialize(ObjectiveData data)
        {
            //set the ID and display the text
            OnTrackedObjectiveAdded.Invoke();
            objectiveID = data.ID;
            objectiveText.text = data.ObjectiveText;
        }
    }
}