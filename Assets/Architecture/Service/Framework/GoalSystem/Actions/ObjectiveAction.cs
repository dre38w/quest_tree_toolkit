
/*
Description: Base class for all objective actions.
*/

using Service.Framework.GoalManagement;
using UnityEngine;
using UnityEngine.Events;

namespace Service.Framework.Goals
{
    public enum ActionState
    {
        Active,
        Inactive,
    }

    public class ObjectiveAction : MonoBehaviour
    {
        public class OnActionCompleteEvent : UnityEvent<ObjectiveAction>
        {

        }
        public OnActionCompleteEvent OnActionCompleted = new OnActionCompleteEvent();

        public ActionState State { get; set; } = ActionState.Inactive;

        /// <summary>
        /// The goal this action is part of
        /// </summary>
        public GoalID ActionGoalID { get; private set; }
        /// <summary>
        /// The quest this action is part of
        /// </summary>
        public QuestID ActionQuestID { get; private set; }
         
        protected bool isComplete { get; set; }

        //if this action is part of a sequence, is it the last one?
        protected bool isFinalInSequence { get; set; }

        protected bool isBranching;
        public bool IsBranching
        {
            get { return isBranching; }
            set { isBranching = value; }
        }

        public ObjectiveSubaction ParentSubaction { get; set; }

        public virtual void InitializeAction()
        {
            
        }

        /// <summary>
        /// Reset the objective action so we can run it again
        /// </summary>
        public virtual void ReinitializeAction()
        {
            isComplete = false;
            isBranching = false;
            SetState(ActionState.Inactive);
        }

        public void SetGoalID(GoalID id)
        {
            ActionGoalID = id;
        }

        public void SetQuestID(QuestID id)
        {
            ActionQuestID = id;
        }

        public virtual void ActionUpdate(float deltaTime)
        {
            if (isComplete && State == ActionState.Inactive)
            {
                return;
            }
        }

        public virtual bool IsComplete()
        {
            return isComplete;
        }

        /// <summary>
        /// The values required to consider the action complete
        /// </summary>
        public virtual void SetComplete()
        {
            isComplete = true;

            SetState(ActionState.Inactive);
            OnActionCompleted.Invoke(this);
        }

        public virtual bool IsFinalInSequence()
        {
            return isFinalInSequence;
        }

        public virtual void SetFinalInSequence(bool state)
        {
            isFinalInSequence = state;
        }

        public virtual void SetState(ActionState currentState)
        {
            //only process if the state is changing to a new state
            if (State == currentState)
            {
                return;
            }

            State = currentState;

            switch (State)
            {
                case ActionState.Active:
                    InitializeAction();
                    break;
            }
        }

        public virtual void ResetValues()
        {

        }
    }
}