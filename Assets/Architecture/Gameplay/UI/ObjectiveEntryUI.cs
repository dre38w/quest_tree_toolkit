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

        private string objectiveID;
        public string ObjectiveID
        {
            get { return objectiveID; }
            set { objectiveID = value; }
        }

        [SerializeField]
        private TMP_Text objectiveText;

        /// <summary>
        /// Can do text enter effects here.
        /// </summary>
        /// <param name="data"></param>
        public void Initialize(ObjectiveData data)
        {
            OnObjectiveAdded.Invoke();
            objectiveID = data.ID;
            SetText(data);
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

    ///****NOTE:  Below is some simple coded effects to show completing the objective****///
            
            //if we want to hide the text
            if (hideComplete)
            {
                gameObject.SetActive(false);
            }
            //if we want to visually complete the text
            else
            {
                //set the text with a strike through
                objectiveText.text = $"<s>{data.ObjectiveText}</s>";
            }
        }

        private void SetText(ObjectiveData data)
        {
            objectiveText.text = data.ObjectiveText;

        }

        public void RefreshEntry(ObjectiveData data, bool hideComplete)
        {
            //not complete, so just display normal
            if (!data.IsComplete)
            {
                SetText(data);
            }
            //is complete
            else
            {
                CompleteEntry(data, hideComplete);
            }
        }
    }
}