
/*
Description: Handles sub actions allowing a completed action to trigger other actions.
Serving as a conditional action.
Able to take both single actions and subactions, creating nesting conditional actions
*/
using Service.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Service.Framework.Goals
{
    [Serializable]
    public class ObjectiveSubaction : ObjectiveAction, INodeContainer
    {        
        /// <summary>
        /// Can use sub and single actions
        /// </summary>
        [Serializable]
        public class ObjectiveActionData
        {
            [Tooltip("The nested action.  Field can take any number of ObjectiveActions and ObjectiveSubactions.")]
            public ObjectiveAction objectiveAction;
        }

        /// <summary>
        /// The current running sub action
        /// </summary>
        private ObjectiveAction currentSubAction;
        public ObjectiveAction CurrentSubaction
        {
            get
            {
                return currentSubAction;
            }
            set
            {
                currentSubAction = value;
            }
        }

        [Tooltip("The list of subactions required to complete in order to complete this conditional action.")]
        [SerializeField]
        private List<ObjectiveAction> objectiveSubactions;
        public List<ObjectiveAction> ObjectiveSubactions
        {
            get
            {
                return objectiveSubactions;
            }
            set
            {
                objectiveSubactions = value;
            }
        }

        public List<ObjectiveAction> GetChildren() => objectiveSubactions;

        [Tooltip("Is the player required to complete the subactions in sequence?")]
        [SerializeField]
        private bool isSequential = true;
        [SerializeField]
        private bool isRandomAtStart = false;

        private void Start()
        {
            for (int i = 0; i < objectiveSubactions.Count; i++)
            {
                objectiveSubactions[i].OnActionCompleted.AddListener(OnSubactionCompleted);
            }
        }

        /// <summary>
        /// Do initialization logic at the start of the goal.
        /// </summary>
        public override void InitializeAction()
        {
            currentSubAction = null;

            foreach (ObjectiveAction action in objectiveSubactions)
            {
                //set this as the parent of the actions in the subaction list
                action.ParentSubaction = this;
                action.SetQuestID(ActionQuestID);

                //now also initialize all actions that are subactions
                if (action is ObjectiveSubaction sub)
                {
                    sub.InitializeAction();
                }
            }

            if (isRandomAtStart)
            {
                ShuffleList.Shuffle(objectiveSubactions);
            }
            if (isSequential)
            {
                //mark the last action in the list as the last one
                objectiveSubactions[objectiveSubactions.Count - 1].SetFinalInSequence(true);
            }
                        
            //activate the first sub action
            SetNextActiveAction(0);
        }

        /// <summary>
        /// Used to restart/reset the action
        /// </summary>
        public override void ReinitializeAction()
        {
            base.ReinitializeAction();

            //reinitialize all the sub actions this script manages
            for (int i = 0; i < objectiveSubactions.Count; i++)
            {
                objectiveSubactions[i].ReinitializeAction();
            }
            currentSubAction = null;
        }

        /// <summary>
        /// Do Update logic
        /// </summary>
        /// <param name="deltaTime"></param>
        public override void ActionUpdate(float deltaTime)
        {
            if (isComplete && State == ActionState.Inactive)
            {
                return;
            }

            //handle calling Update on all sub actions
            for (int actionIndex = 0; actionIndex < objectiveSubactions.Count; actionIndex++)
            {
                ObjectiveAction objectiveAction = objectiveSubactions[actionIndex];
                if (!objectiveAction.IsComplete())
                {
                    objectiveAction.ActionUpdate(deltaTime);
                }
            }
            CheckCompleteActions();
        }

        public override void ForceCompleteAction()
        {
            base.ForceCompleteAction();

            for (int i = 0; i < objectiveSubactions.Count; i++)
            {
                objectiveSubactions[i].ForceCompleteAction();
            }
            currentSubAction = null;
        }

        /// <summary>
        /// Handle evaluating the completion status of the actions
        /// </summary>
        private void CheckCompleteActions()
        {
            //after all sub actions are complete, we can now complete the action
            if (objectiveSubactions.All(a => a.IsComplete()))
            {
                isComplete = true;
                OnActionCompleted.Invoke(this);
                ResetValues();
                return;
            }

            //if more to complete, then move to the next sub action
            MoveToNextAction();
        }

        /// <summary>
        /// Move to the next action in the list
        /// </summary>
        private void MoveToNextAction()
        {
            //if the current action is completed, move to the next one
            if (currentSubAction && currentSubAction.IsComplete())
            {
                int nextSubactionIndex = objectiveSubactions.FindIndex(data => data == currentSubAction);
                SetNextActiveAction(nextSubactionIndex + 1);
                //reset to 0 to avoid potential garbage data
                nextSubactionIndex = 0;
            }
        }

        /// <summary>
        /// Handle activating the next action we are wanting to start
        /// </summary>
        /// <param name="subActionIndex"></param>
        private void SetNextActiveAction(int subActionIndex)
        {
            //if this is a non sequential action, activate all actions to allow completing them in any order
            if (!isSequential)
            {
                for (int actionIndex = 0; actionIndex < objectiveSubactions.Count; actionIndex++)
                {
                    objectiveSubactions[actionIndex].SetState(ActionState.Active);
                }
            }
            //otherwise, activate the next in the sub action sequence
            else
            {
                if (subActionIndex < objectiveSubactions.Count)
                {
                    currentSubAction = objectiveSubactions[subActionIndex];
                    currentSubAction.SetState(ActionState.Active);
                }
            }
        }

        public virtual bool HandleBranch(ObjectiveAction sourceAction)
        {
            return false;
        }

        /// <summary>
        /// Listener method called when one of this class's subactions completes
        /// </summary>
        /// <param name="action"></param>
        private void OnSubactionCompleted(ObjectiveAction action)
        {
            if (!action.IsBranching)
            {
                return;
            }

            //safety check to make sure we are calling on a BranchingSubaction
            if (HandleBranch(action))
            {
                return;
            }

            //if the parent of the action was this subaction then we know it is a BranchingSubaction class
            //and it needs to handle the branching logic in its override method
            ParentSubaction.HandleBranch(this);
        }

        /// <summary>
        /// Used to force complete all the sub actions.
        /// Useful when handling branching, restart/reset logic, etc.
        /// </summary>
        public void ForceCompleteSubactions()
        {
            for (int i = 0; i < objectiveSubactions.Count; i++)
            {
                objectiveSubactions[i].SetComplete();
            }
        }

        public override void ResetValues()
        {
            SetState(ActionState.Inactive);
            currentSubAction = null;
        }

        private void OnDestroy()
        {
            for (int i = 0; i < objectiveSubactions.Count; i++)
            {
                objectiveSubactions[i].OnActionCompleted.RemoveListener(OnSubactionCompleted);
            }
        }
    }
}