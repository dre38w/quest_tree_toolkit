
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
    public class Goal : MonoBehaviour
    {
        public class OnCompleteGoalEvent : UnityEvent<Goal> { }
        public OnCompleteGoalEvent OnCompleteGoal = new OnCompleteGoalEvent();

        public GoalID id;

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

            //TODO:  only here for testing
            InitializeGoal();
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

        public virtual void ReinitializeGoal()
        {
            isComplete = false;

            for (int actionIndex = 0; actionIndex < objectiveActions.Count; actionIndex++)
            {
                objectiveActions[actionIndex].ReinitializeAction();
            }
            InitializeGoal();
        }

        private void CheckActionsComplete(ObjectiveAction action)
        {
            //all actions are complete.
            //we can now complete the goal
            if (objectiveActions.All(a => a.IsComplete()))
            {
                isComplete = true;
                OnCompleteGoal.Invoke(this);
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

        public virtual bool IsComplete()
        {
            return isComplete;
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