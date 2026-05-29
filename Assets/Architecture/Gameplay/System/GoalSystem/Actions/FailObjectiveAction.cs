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

        [Tooltip("Do we need to target a specific objective to fail or the last added objective?")]
        [SerializeField]
        private bool isTargettedObjective;

        [Tooltip("The objective we want to perma fail.")]
        [SerializeField]
        private UpdateObjectiveLog targetObjective;

        [Tooltip("How long before completing the action?")]
        [SerializeField]
        private float completeActionWaitTime = 1f;

        private WaitForSeconds completeActionWait;

        private void Start()
        {
            completeActionWait = new WaitForSeconds(completeActionWaitTime);
        }

        public override void InitializeAction()
        {
            if (targetObjective == null)
            {
                return;
            }
            GoalManager.Instance.GoalTracker.MarkObjectiveFailed(ActionQuestID, targetObjective.CreatedObjectiveID, isPermaFail);
            OnObjectiveFailed.Invoke();

            //if (isPermaFail)
            //{
            //    //if (targetObjective != null)
            //    //{
            //    //    string objectiveID = targetObjective.CreatedObjectiveID;
            //    //    GoalManager.Instance.GoalTracker.MarkObjectiveComplete(ActionQuestID, objectiveID);
            //    //}
            //    SetComplete();
            //}
            //else
            //{
                StartCoroutine(ResetObjectiveWaitTimer(targetObjective.CreatedObjectiveID));
            //}
            //OnObjectiveFailed.Invoke();

            //if (isTargettedObjective)
            //{
            //    StartCoroutine(ResetObjectiveWaitTimer(targetObjective.CreatedObjectiveID));
            //}
            //else
            //{
            //    //
            //}
        }

        private IEnumerator ResetObjectiveWaitTimer(string objectiveID)
        {
            yield return completeActionWait;

            if (!isPermaFail)
            {
                GoalManager.Instance.GoalTracker.RestartObjective(ActionQuestID, objectiveID);

            }
            //OnObjectiveRestart.Invoke();
            SetComplete();
        }
    }
}