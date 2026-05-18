using Service.Core;
using Service.Framework.GoalManagement;
using Service.Framework.Goals;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.System.Actions
{
    [Submenu("Quest Log Management/Fail Objective")]
    public class FailObjectiveAction : ObjectiveAction
    {
        /*
         * TODO:  add the ability to fail last added objective
         */
        public UnityEvent OnObjectiveFailed = new UnityEvent();
        public UnityEvent OnObjectiveRestart = new UnityEvent();

        [Tooltip("Should the objective remain in failed state indefinitely?")]
        [SerializeField]
        private bool isPermaFail;

        [Tooltip("The objective we want to perma fail.")]
        [SerializeField]
        private UpdateObjectiveLog targetObjective;

        [Tooltip("How long before restarting the objective if perma fail is false?")]
        [SerializeField]
        private float resetObjectiveWaitTime = 1f;

        private WaitForSeconds resetObjectiveWait;

        private void Start()
        {
            resetObjectiveWait = new WaitForSeconds(resetObjectiveWaitTime);
        }

        public override void InitializeAction()
        {
            if (targetObjective == null)
            {
                return;
            }
            GoalManager.Instance.GoalTracker.MarkObjectiveFailed(ActionQuestID, targetObjective.CreatedObjectiveID, isPermaFail);
            OnObjectiveFailed.Invoke();

            if (isPermaFail)
            {
                //if (targetObjective != null)
                //{
                //    string objectiveID = targetObjective.CreatedObjectiveID;
                //    GoalManager.Instance.GoalTracker.MarkObjectiveComplete(ActionQuestID, objectiveID);
                //}
                SetComplete();
            }
            else
            {
                StartCoroutine(ResetObjectiveWaitTimer(targetObjective.CreatedObjectiveID));
            }
            //OnObjectiveFailed.Invoke();
        }

        private IEnumerator ResetObjectiveWaitTimer(string objectiveID)
        {
            yield return resetObjectiveWait;
            GoalManager.Instance.GoalTracker.RestartObjective(ActionQuestID, objectiveID);
            OnObjectiveRestart.Invoke();
            SetComplete();
        }
    }
}