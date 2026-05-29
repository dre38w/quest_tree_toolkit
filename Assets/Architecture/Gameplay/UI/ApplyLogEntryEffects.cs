using Service.Framework;
using Service.Framework.Goals;
using System.Collections;
using UnityEngine;

namespace Gameplay.UI
{
    public class ApplyLogEntryEffects : MonoBehaviour
    {
        #region Normal Objectives
        /// <summary>
        /// Handle displaying an active/incomplete objective in the objective log
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="data"></param>
        public void ApplyObjectiveNormal(ObjectiveEntryUI entry, ObjectiveData data)
        {
            entry.gameObject.SetActive(true);
            entry.ObjectiveText.text = data.ObjectiveText;
        }

        /// <summary>
        /// Handle complete objective
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="data"></param>
        /// <param name="hideComplete"></param>
        public void ApplyObjectiveComplete(ObjectiveEntryUI entry, ObjectiveData data, bool hideComplete)
        {
            //NOTE:  Edit this line to call your desired effect
            entry.ObjectiveText.text = UIEffects.ApplyStrikeThrough(entry.ObjectiveText.text, data.ObjectiveText);

            if (hideComplete)
            {
                StartCoroutine(ObjectiveUIHideDelay(entry, entry.StateChangeDelay));                
                return;
            }
            entry.gameObject.SetActive(true); //may not need this line
        }
        #endregion

        /// <summary>
        /// Handle tracked objective
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="data"></param>
        public void ApplyTrackedObjectiveNormal(ObjectiveTrackerUI entry, ObjectiveData data)
        {
            entry.gameObject.SetActive(true);
            entry.ObjectiveText.text = data.ObjectiveText;
        }

        /// <summary>
        /// Handle tracked complete objective
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="data"></param>
        /// <param name="hideComplete"></param>
        public void ApplyTrackedObjectiveComplete(ObjectiveTrackerUI entry, ObjectiveData data)
        {
            //NOTE:  Edit this line to call your desired effect
            entry.ObjectiveText.text = UIEffects.ApplyStrikeThrough(entry.ObjectiveText.text, data.ObjectiveText);

            StartCoroutine(TrackedObjectiveUIHideDelay(entry, entry.StateChangeDelay));

        }

        /// <summary>
        /// Handle failed objective
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="data"></param>
        public void ApplyObjectiveFailed(ObjectiveEntryUI entry, ObjectiveData data, bool hideComplete)
        {
            //NOTE:  Edit these lines to call your desired effect
            entry.ObjectiveText.text = UIEffects.ApplyColor(entry.ObjectiveText.text, data.ObjectiveText, "red");
            entry.ObjectiveText.text = UIEffects.ApplySubtext(entry.ObjectiveText.text, LogEntryEffectData.OBJECTIVE_FAIL_TEXT);

            if (hideComplete)
            {
                StartCoroutine(ObjectiveUIHideDelay(entry, entry.StateChangeDelay));
                return;
            }
            entry.gameObject.SetActive(true);
        }

        /// <summary>
        /// Handle failed tracked objective
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="data"></param>
        public void ApplyTrackedObjectiveFailed(ObjectiveTrackerUI entry, ObjectiveData data)
        {
            //NOTE:  Edit these lines to call your desired effect
            entry.ObjectiveText.text = UIEffects.ApplyColor(entry.ObjectiveText.text, data.ObjectiveText, "red");
            entry.ObjectiveText.text = UIEffects.ApplySubtext(entry.ObjectiveText.text, LogEntryEffectData.OBJECTIVE_FAIL_TEXT);

            StartCoroutine(TrackedObjectiveUIHideDelay(entry, entry.StateChangeDelay));
        }

        public void ApplyQuestNormal(QuestEntryUI entry, QuestID id)
        {
            entry.gameObject.SetActive(true);
            entry.QuestTitle.text = id.questName;
        }

        public void ApplyQuestComplete(QuestEntryUI entry, QuestID id, bool hideComplete)
        {
            //NOTE:  Edit this line to call your desired effect
            entry.QuestTitle.text = UIEffects.ApplyStrikeThrough(entry.QuestTitle.text, id.questName);

            if (hideComplete)
            {
                StartCoroutine(QuestUIHideDelay(entry, entry.StateChangeDelay));
                return;
            }
            entry.gameObject.SetActive(true);
        }

        private IEnumerator ObjectiveUIHideDelay(ObjectiveEntryUI entry, float delay)
        {
            yield return new WaitForSeconds(delay);
            entry.gameObject.SetActive(false);
        }

        private IEnumerator TrackedObjectiveUIHideDelay(ObjectiveTrackerUI entry, float delay)
        {
            yield return new WaitForSeconds(delay);
            entry.gameObject.SetActive(false);
        }

        private IEnumerator QuestUIHideDelay(QuestEntryUI entry, float delay)
        {
            yield return new WaitForSeconds(delay);
            entry.gameObject.SetActive(false);
        }
    }
}