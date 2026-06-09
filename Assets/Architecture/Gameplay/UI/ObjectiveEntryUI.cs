/*
 * Description: Handles displaying the objective on the quest log UI
 */
using Service.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.UI
{
    public class ObjectiveEntryUI : MonoBehaviour
    {
        [Tooltip("Use this event to do effects when the objective is added.")]
        public UnityEvent OnObjectiveAdded = new UnityEvent();
        [Tooltip("Use this event to do effects when the objective is completed.")]
        public UnityEvent OnObjectiveCompleted = new UnityEvent();
        [Tooltip("Use this event to do effects when the objective fails.")]
        public UnityEvent OnObjectiveFailed = new UnityEvent();

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
        /// Can do text enter effects here.
        /// </summary>
        /// <param name="data"></param>
        public void Initialize(ObjectiveData data)
        {
            OnObjectiveAdded.Invoke();
            objectiveID = data.ID;
            objectiveText.text = data.ObjectiveText;

            //StartCoroutine(UIEffects.StateDelay(objectiveText.text, data.ObjectiveText));
        }

        /// <summary>
        /// Can do text exit effects here
        /// </summary>
        /// <param name="data"></param>
        /// <param name="hideComplete"></param>
        public void CompleteEntry(ObjectiveData data, bool hideComplete)
        {
            //can also use this event in the inspector to call your effects
            OnObjectiveCompleted.Invoke();
        }

        public void ObjectiveFailed()
        {
            OnObjectiveFailed.Invoke();
        }
    }
}