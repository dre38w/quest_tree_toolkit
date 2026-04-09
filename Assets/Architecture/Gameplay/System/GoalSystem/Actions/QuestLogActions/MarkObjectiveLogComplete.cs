/*
 * Description: Marks an objective complete.
 */
using Service.Framework.GoalManagement;
using Service.Framework.Goals;
using UnityEngine;

namespace Gameplay.System.Actions
{
    public class MarkObjectiveLogComplete : ObjectiveAction
    {
        [Tooltip("Does this objective complete the quest?")]
        [SerializeField]
        private bool doesCompleteQuest;

        public override void InitializeAction()
        {
            GoalManager.Instance.GoalTracker.CompleteLatestObjective(ActionQuestID);
            //does this objective also complete the quest its associated with?
            if (doesCompleteQuest)
            {
                GoalManager.Instance.GoalTracker.CompleteQuest(ActionQuestID);
            }
            SetComplete();
        }
    }
}