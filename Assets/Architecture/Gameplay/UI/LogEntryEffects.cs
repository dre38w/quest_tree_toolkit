using Service.Framework;
using Service.Framework.Goals;
using TMPro;
using UnityEngine;

/*
 * TODO:  add an array so the effects can stack
 */
namespace Gameplay.UI
{
    public class LogEntryEffects : MonoBehaviour
    {
        public void ApplyObjectiveNormal(ObjectiveEntryUI entry, ObjectiveData data)
        {
            entry.gameObject.SetActive(true);
            entry.ObjectiveText.text = data.ObjectiveText;
        }

        public void ApplyObjectiveComplete(ObjectiveEntryUI entry, ObjectiveData data, bool hideComplete)
        {
            if (hideComplete)
            {
                entry.gameObject.SetActive(false);
                return;
            }
            entry.gameObject.SetActive(true);

            //call the effect to apply here
            entry.ObjectiveText.text = ApplyStrikeThrough(entry.ObjectiveText.text, data.ObjectiveText);
        }

        public void ApplyObjectiveFailed(ObjectiveEntryUI entry, ObjectiveData data)
        {
            entry.gameObject.SetActive(true);

            //call the effect to apply here
            entry.ObjectiveText.text = ApplyColor(entry.ObjectiveText.text, data.ObjectiveText, "red");
        }

        public void ApplyTrackedObjectiveNormal(ObjectiveTrackerUI entry, ObjectiveData data)
        {
            entry.gameObject.SetActive(true);
            entry.ObjectiveText.text = data.ObjectiveText;
        }

        public void ApplyTrackedObjectiveComplete(ObjectiveTrackerUI entry, ObjectiveData data, bool hideComplete)
        {
            if (hideComplete)
            {
                entry.gameObject.SetActive(false);
                return;
            }
            entry.gameObject.SetActive(true);

            //call the effect to apply here
            entry.ObjectiveText.text = ApplyStrikeThrough(entry.ObjectiveText.text, data.ObjectiveText);
        }

        public void ApplyTrackedObjectiveFailed(ObjectiveTrackerUI entry, ObjectiveData data)
        {
            entry.gameObject.SetActive(true);

            //call the effect to apply here
            entry.ObjectiveText.text = ApplyColor(entry.ObjectiveText.text, data.ObjectiveText, "red");
        }


        public void ApplyQuestNormal(QuestEntryUI entry, QuestID id)
        {
            entry.gameObject.SetActive(true);
            entry.QuestTitle.text = id.questName;
        }

        public void ApplyQuestComplete(QuestEntryUI entry, QuestID id, bool hideComplete)
        {
            if (hideComplete)
            {
                entry.gameObject.SetActive(false);
                return;
            }
            entry.gameObject.SetActive(true);
            entry.QuestTitle.text = ApplyStrikeThrough(entry.QuestTitle.text, id.questName);

        }

        #region Effects
        private string ApplyStrikeThrough(string textToEffect, string displayText)
        {
            return textToEffect = $"<s>{displayText}</s>";
        }

        private string ApplyColor(string textToEffect, string displayText, string color)
        {
            return textToEffect = $"<color={color}>{displayText}</color>";
        }
        #endregion
    }
}