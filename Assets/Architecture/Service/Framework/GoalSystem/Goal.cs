
/*
Description: Holds a list of objective actions and checks when they have all complete
*/

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using Service.Core.Utilities;

namespace Service.Framework.Goals
{
    public enum GoalState
    {
        Active,
        Inactive,
    }
    public class Goal : MonoBehaviour
    {
        public class OnCompleteGoalEvent : UnityEvent<Goal> { }
        public OnCompleteGoalEvent OnCompleteGoal = new OnCompleteGoalEvent();

        public GoalState State { get; set; } = GoalState.Inactive;

        public GoalID id;

        [Tooltip("Conditions to be met before the Goal can start.")]
        [SerializeReference]
        internal List<GoalRequirement> requirements = new List<GoalRequirement>();
        public List<GoalRequirement> Requirements => requirements;

        [Tooltip("The Objective Actions required to complete in order to complete the Goal.")]
        [SerializeField]
        private List<ObjectiveAction> objectiveActions = new List<ObjectiveAction>();
        public List<ObjectiveAction> ObjectiveActions
        {
            get
            {
                return objectiveActions;
            }
            set
            {
                objectiveActions = value;
            }
        }
        private ObjectiveAction currentObjectiveAction;
        public ObjectiveAction CurrentObjectiveAction
        {
            get
            {
                return currentObjectiveAction;
            }
            set
            {
                currentObjectiveAction = value;
            }
        }

        private bool isComplete;
        [Tooltip("Should the order of the actions randomize when the goal begins?")]
        [SerializeField]
        private bool isRandomAtStart;
        [Tooltip("Is the player required to complete the actions in order?")]
        [SerializeField]
        private bool isSequential = true;

        private void Start()
        {
            if (objectiveActions.Count == 0)
            {
                Debug.LogError($"{objectiveActions} list is null.  Please populate via Inspector.", gameObject);
            }

            GoalManager.AddGoal(this);
            for (int actionIndex = 0; actionIndex < objectiveActions.Count; actionIndex++)
            {
                objectiveActions[actionIndex].OnActionCompleted.AddListener(CheckActionsComplete);
            }
        }

        private void OnValidate()
        {
            ValidateRequirements();
        }

        private void ValidateRequirements()
        {
            if (requirements == null)
            {
                return;
            }

            for (int i = 0; i < requirements.Count; i++)
            {
                if (requirements[i] == null)
                {
                    Debug.LogWarning($"Missing GoalRequirement reference at index {i}", this);
                }
            }
        }

        public virtual void InitializeGoal()
        {
            isComplete = false;

            if (isRandomAtStart)
            {
                ShuffleList.Shuffle(objectiveActions);
            }

            //if this is a non sequential goal, activate all actions to allow completing them in any order
            if (!isSequential)
            {
                for (int actionIndex = 0; actionIndex < objectiveActions.Count; actionIndex++)
                {
                    objectiveActions[actionIndex].SetState(ActionState.Active);
                }
            }
            //otherwise, activate the first one in the list
            else
            {
                currentObjectiveAction = objectiveActions[0];
                objectiveActions[0].SetState(ActionState.Active);
            }
        }

        /// <summary>
        /// In the event we want to restart or reset this goal
        /// </summary>
        public virtual void ReinitializeGoal()
        {
            isComplete = false;

            //reset all the actions
            for (int actionIndex = 0; actionIndex < objectiveActions.Count; actionIndex++)
            {
                objectiveActions[actionIndex].ReinitializeAction();
            }
            //reset state
            SetState(GoalState.Inactive);
        }

        private void CheckActionsComplete(ObjectiveAction action)
        {
            //all actions are complete.
            //we can now complete the goal
            if (objectiveActions.All(a => a.IsComplete()))
            {
                SetComplete();
                return;
            }
            //if more actions to complete, move to the next one
            if (isSequential)
            {
                int nextActionIndex = objectiveActions.IndexOf(action);
                currentObjectiveAction = objectiveActions[nextActionIndex + 1];
                currentObjectiveAction.SetState(ActionState.Active);
            }
        }

        public virtual void GoalUpdate(float deltaTime)
        {
            if (isComplete)
            {
                return;
            }

            //check the requirements
            if (State == GoalState.Inactive)
            {
                CheckStartRequirements(deltaTime);
                return;
            }
            
            //call Update methods on the actions
            for (int actionIndex = 0; actionIndex < objectiveActions.Count; actionIndex++)
            {
                ObjectiveAction objectiveAction = objectiveActions[actionIndex];
                if (!objectiveAction.IsComplete())
                {
                    objectiveAction.ActionUpdate(deltaTime);
                }
            }
        }

        public virtual void SetComplete()
        {
            isComplete = true;
            OnCompleteGoal.Invoke(this);
        }

        public virtual void CheckStartRequirements(float deltaTime)
        {
            //if the requirements are met, we can start the goal
            if (IsRequirementsMet())
            {
                SetState(GoalState.Active);
                InitializeGoal();
            }
        }

        /// <summary>
        /// Check to see if all the requirements are met
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRequirementsMet()
        {
            if (requirements.All(req => req.IsRequirementMet(this)))
            {
                return true;
            }
            return false;
        }

        public virtual bool IsComplete()
        {
            return isComplete;
        }

        public virtual void SetState(GoalState currentState)
        {
            //only process if the state is changing to a new state
            if (State == currentState)
            {
                return;
            }

            State = currentState;
        }

        private void OnDestroy()
        {
            for (int actionIndex = 0; actionIndex < objectiveActions.Count; actionIndex++)
            {
                objectiveActions[actionIndex].OnActionCompleted.RemoveListener(CheckActionsComplete);
            }
        }
    }
}