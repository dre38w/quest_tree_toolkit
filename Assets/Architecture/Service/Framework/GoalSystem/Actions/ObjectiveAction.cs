
/*
Description: Base class for all objective actions.
*/

using UnityEngine;
using UnityEngine.Events;
using Service.Framework.GoalManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif

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

        [Tooltip("Should this objective be added to the tracking database?")]
        [SerializeField]
        private bool isTracked;
        public bool IsTracked => isTracked;

        [SerializeField]
        private ObjectiveID actionID;
        public ObjectiveID ActionID => actionID;

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

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || Application.isPlaying)
            {
                return;
            }
            if (!isTracked || actionID != null)
            {
                return;
            }
            string ownerID = GetEditorOwnerID();

            if (string.IsNullOrEmpty(ownerID))
            {
                return;
            }


            CreateObjectiveID(ownerID);
            //if (string.IsNullOrEmpty(actionID))
            //{
            //    actionID = Guid.NewGuid().ToString();
            //    UnityEditor.EditorUtility.SetDirty(this);
            //}
        }

        public string GetEditorOwnerID()
        {
            GlobalObjectId globalID = GlobalObjectId.GetGlobalObjectIdSlow(this);

            string id = globalID.ToString();

            if (string.IsNullOrEmpty(id) || id.Contains("0000000000000000000"))
            {
                return null;
            }
            return id;
        }

        public void CreateObjectiveID(string ownerID)
        {
            if (actionID != null)
            {
                return;
            }
            const string folder = "Assets/GeneratedObjectiveIDs"; //change the path and move the variable

            if (!AssetDatabase.IsValidFolder(folder))
            {
                AssetDatabase.CreateFolder("Assets", "GeneratedObjectiveIDs");
            }

            ObjectiveID newID = ScriptableObject.CreateInstance<ObjectiveID>();
            newID.SetOwner(ownerID);

            string idName = gameObject.name.Replace("/", "_");
            string path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{idName}_Objective.asset");

            AssetDatabase.CreateAsset(newID, path);
            AssetDatabase.SaveAssets();

            Undo.RecordObject(this, "Create Objectived ID");

            actionID = newID;

            EditorUtility.SetDirty(this);
        }
#endif

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
            RegisterTrackedObjective();
        }

        protected virtual void RegisterTrackedObjective()
        {
            if (!isTracked)
            {
                return;
            }
            if (actionID == null)
            {
                return;
            }

            GoalManager.Instance.GoalTracker.AddTrackedObjective(ActionQuestID, actionID);
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
        /// Force the objective into complete status
        /// </summary>
        public virtual void ForceCompleteAction()
        {
            SetComplete();
        }

        /// <summary>
        /// The values required to consider the action complete
        /// </summary>
        public virtual void SetComplete()
        {
            isComplete = true;

            if (isTracked && actionID != null)
            {
                GoalManager.Instance.GoalTracker.MarkObjectiveComplete(ActionQuestID, actionID);
            }

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