using Service.Framework;
using Service.Framework.Goals;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.UI
{
    public class ApplyLogEntryEffects : MonoBehaviour
    {
        public UnityEvent OnTrackedObjectiveHidden = new UnityEvent();
        public UnityEvent OnTrackedQuestHidden = new UnityEvent();
        public UnityEvent OnQuestHidden = new UnityEvent();
        public UnityEvent OnObjectiveHidden = new UnityEvent();

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

        /// <summary>
        /// Handle tracked objective
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="data"></param>
        public void ApplyTrackedObjectiveNormal(ObjectiveTrackerUI entry, ObjectiveData data)
        {
            entry.ObjectiveText.text = data.ObjectiveText;
        }

        /// <summary>
        /// Handle tracked complete objective
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="data"></param>
        /// <param name="hideComplete"></param>
        public void ApplyTrackedObjectiveComplete(ObjectiveTrackerUI entry, ObjectiveData data, bool hideComplete = true)
        {
            //NOTE:  Edit this line to call your desired effect
            entry.ObjectiveText.text = UIEffects.ApplyStrikeThrough(entry.ObjectiveText.text, data.ObjectiveText);

            if (hideComplete)
            {
                StartCoroutine(TrackedObjectiveUIHideDelay(entry, entry.StateChangeDelay));
            }
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
        public void ApplyTrackedObjectiveFailed(ObjectiveTrackerUI entry, ObjectiveData data, bool hideComplete = true)
        {
            //NOTE:  Edit these lines to call your desired effect
            entry.ObjectiveText.text = UIEffects.ApplyColor(entry.ObjectiveText.text, data.ObjectiveText, "red");
            entry.ObjectiveText.text = UIEffects.ApplySubtext(entry.ObjectiveText.text, LogEntryEffectData.OBJECTIVE_FAIL_TEXT);

            if (hideComplete)
            {
                StartCoroutine(TrackedObjectiveUIHideDelay(entry, entry.StateChangeDelay));
            }
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

        public void ApplyQuestFailed(QuestEntryUI entry, QuestData data, bool hideComplete)
        {
            entry.QuestTitle.text = UIEffects.ApplyColor(entry.QuestTitle.text, entry.QuestTitle.text, "red");
            entry.QuestTitle.text = UIEffects.ApplySubtext(entry.QuestTitle.text, LogEntryEffectData.OBJECTIVE_FAIL_TEXT);

            if (hideComplete)
            {
                StartCoroutine(QuestUIHideDelay(entry, entry.StateChangeDelay));
            }
        }

        public void ApplyTrackedQuestNormal(TrackedQuestEntryUI entry, QuestID id)
        {
            entry.gameObject.SetActive(true);
            entry.QuestTitle.text = id.questName;
        }

        public void ApplyTrackedQuestComplete(TrackedQuestEntryUI entry, QuestID id, bool hideComplete = true)
        {
            //NOTE:  Edit this line to call your desired effect
            entry.QuestTitle.text = UIEffects.ApplyStrikeThrough(entry.QuestTitle.text, id.questName);

            if (hideComplete)
            {
                StartCoroutine(TrackedQuestUIHideDelay(entry, entry.StateChangeDelay));
                return;
            }
            entry.gameObject.SetActive(true);
        }

        public void ApplyTrackedQuestFailed(TrackedQuestEntryUI entry, QuestData data, bool hideComplete = true)
        {
            entry.QuestTitle.text = UIEffects.ApplyColor(entry.QuestTitle.text, entry.QuestTitle.text, "red");
            entry.QuestTitle.text = UIEffects.ApplySubtext(entry.QuestTitle.text, LogEntryEffectData.OBJECTIVE_FAIL_TEXT);

            if (hideComplete)
            {
                StartCoroutine(TrackedQuestUIHideDelay(entry, entry.StateChangeDelay));
            }
        }

        private IEnumerator ObjectiveUIHideDelay(ObjectiveEntryUI entry, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (entry != null)
            {
                entry.gameObject.SetActive(false);
            }
            OnObjectiveHidden.Invoke();
        }

        private IEnumerator TrackedObjectiveUIHideDelay(ObjectiveTrackerUI entry, float delay)
        {
            yield return new WaitForSeconds(delay);

            //check that the objective is not null, since we spawn the objective under the quest
            //there are instances where we destroy the quest before destroying the objective
            if (entry != null)
            {
                entry.gameObject.SetActive(false);
            }
            OnTrackedObjectiveHidden.Invoke();
        }

        private IEnumerator QuestUIHideDelay(QuestEntryUI entry, float delay)
        {
            yield return new WaitForSeconds(delay);
            entry.gameObject.SetActive(false);
            OnQuestHidden.Invoke();
        }

        private IEnumerator TrackedQuestUIHideDelay(TrackedQuestEntryUI entry, float delay)
        {
            yield return new WaitForSeconds(delay);
            entry.gameObject.SetActive(false);
            OnTrackedQuestHidden.Invoke();
        }
    }
}