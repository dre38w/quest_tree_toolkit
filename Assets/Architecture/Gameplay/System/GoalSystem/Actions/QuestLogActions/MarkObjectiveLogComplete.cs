/*
 * Description: Marks an objective complete.
 */
using NUnit.Framework;
using Service.Core;
using Service.Framework;
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

        public override void InitializeAction()
        {
            if (doesCompleteTargetObjective)
            {
                if (targetObjective != null)
                {
                    string objectiveID = targetObjective.CreatedObjectiveID;
                    GoalManager.Instance.GoalTracker.MarkObjectiveComplete(ActionQuestID, objectiveID);
                }
            }
            else
            {
                GoalManager.Instance.GoalTracker.CompleteLatestObjective(ActionQuestID);
            }
            //does this objective also complete the quest its associated with?
            if (doesCompleteQuest)
            {
                GoalManager.Instance.GoalTracker.CompleteQuest(ActionQuestID);
            }
            SetComplete();
        }
    }
}