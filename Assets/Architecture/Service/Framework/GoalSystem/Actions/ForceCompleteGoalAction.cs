using Service.Framework.GoalManagement;
using UnityEngine;
using UnityEngine.Events;

namespace Service.Framework.Goals
{
    public class ForceCompleteGoalAction : ObjectiveAction
    {
        public UnityEvent<QuestID> OnQuestFailed = new UnityEvent<QuestID>();

        [SerializeField]
        private GoalID[] goalsToComplete;

        [Tooltip("Is this an action that fails the quest?")]
        [SerializeField]
        private bool isFailCondition;

        public override void InitializeAction()
        {
            if (isFailCondition)
            {
                OnQuestFailed.Invoke(ActionQuestID);
            }

            for (int i = 0; i < goalsToComplete.Length; i++)
            {
                GoalManager.Instance.GetGoal(goalsToComplete[i]).ForceCompleteGoal();
            }
            SetComplete();
        }
    }
}