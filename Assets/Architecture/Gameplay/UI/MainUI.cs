/*
 * Description: Handles main UI logic
 */

using Service.Framework.GoalManagement;
using Service.Framework.Goals;
using TMPro;
using UnityEngine;

namespace Gameplay.UI
{
    public class MainUI : MonoBehaviour
    {
        public TMP_Text displayText;
        public bool appendMode;

        [SerializeField]
        private GameObject contextualButtonUi;
        public GameObject ContextualButtonUI
        {
            get { return contextualButtonUi; }
            set { contextualButtonUi = value; }
        }

        private void Start()
        {
            GoalManager.Instance.GoalTracker.OnObjectivesChanged.AddListener(OnUpdateObjectives);
        }

        private void OnUpdateObjectives(GoalID goal)
        {
            var objectives = GoalManager.Instance.GoalTracker.GetObjectives(goal);

            if (objectives == null || objectives.Count == 0)
            {
                return;
            }
            if (appendMode)
            {
                displayText.text = "";

                foreach (var objective in objectives)
                {
                    displayText.text += FormatObjectiveEntry(objective) + "\n";
                }
            }
            else
            {
                var latestEntry = objectives[objectives.Count - 1];
                displayText.text = FormatObjectiveEntry(latestEntry);
            }
        }

        private string FormatObjectiveEntry(ObjectiveData data)
        {
            return data.IsComplete ? $"<s>{data.ObjectiveText}</s>" : data.ObjectiveText;
        }

        /// <summary>
        /// Toggles on or off the contextual action button.
        /// </summary>
        /// <param name="state"></param>
        public void SetContextualUiVisible(bool state)
        {
            contextualButtonUi.SetActive(state);
        }
    }
}