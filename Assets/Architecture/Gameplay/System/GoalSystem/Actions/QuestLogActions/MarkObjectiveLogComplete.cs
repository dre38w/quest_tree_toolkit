/*
 * Description: Marks an objective complete.
 */
using Service.Core;
using Service.Framework.GoalManagement;
using Service.Framework.Goals;
using UnityEngine;

namespace Gameplay.System.Actions
{
    [Submenu("Quest Log Management/Mark Objective Complete")]
    public class MarkObjectiveLogComplete : ObjectiveAction
    {
        [SerializeField]
        private UpdateObjectiveLog targetObjective;

        [Tooltip("True = This targets a specific objective to complete\n" +
            "False = This completes the previous entry")]
        [SerializeField]
        private bool doesCompleteTargetObjective;

        [Tooltip("Does this objective complete the quest?")]
        [SerializeField]
        private bool doesCompleteQuest;

        [Tooltip("If this is a parent objective, does it complete itself after completing all sub objectives?")]
        [SerializeField]
        private bool doesCompleteViaSubObjectives = true;

        public override void InitializeAction()
        {
            if (doesCompleteTargetObjective)
            {
                if (targetObjective != null)
                {
                    GoalManager.Instance.GoalTracker.MarkObjectiveComplete(ActionQuestID, targetObjective.CreatedObjectiveID, doesCompleteViaSubObjectives);
                }
            }
            else
            {
                GoalManager.Instance.GoalTracker.CompleteLatestObjective(ActionQuestID);
            }
            //does this objective also complete the quest it's associated with?
            if (doesCompleteQuest)
            {
                GoalManager.Instance.GoalTracker.CompleteQuest(ActionQuestID);
            }
            SetComplete();
        }
    }
}