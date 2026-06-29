
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

        [Tooltip("Should this objective be recorded in the database?")]
        [SerializeField]
        private bool isRecorded;
        public bool IsRecorded => isRecorded;

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
        /// <summary>
        /// Automatically create an SO for this objective action
        /// </summary>
        protected virtual void OnValidate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || Application.isPlaying)
            {
                return;
            }

            //validate the Inspector based ownership
            ValidateInspectorIdOwnership();

            //if we aren't currently tracking or we already assigned an action ID, early out
            if (!isRecorded || actionID != null)
            {
                return;
            }
            string ownerID = GetEditorOwnerID();

            //invalid owner ID, early out
            if (string.IsNullOrEmpty(ownerID))
            {
                return;
            }
            //no action ID was found, so create one
            CreateObjectiveID(ownerID);
        }

        /// <summary>
        /// Validate the objective ID ownership via Inspector changes
        /// </summary>
        public void ValidateInspectorIdOwnership()
        {
            //action ID is already null, so no checking of ownership is needed.
            if (actionID == null)
            {
                return;
            }
            //get the ownership ID
            string ownerID = GetEditorOwnerID();

            //if null or empty, then the ID is invalid
            if (string.IsNullOrEmpty(ownerID))
            {
                return;
            }

            //if id is the same, early out as this ObjectiveAction is the owner
            if (actionID.OwnerID == ownerID)
            {
                return;
            }
            //this ObjectiveAction is not the owner, so clear some data
            ClearIdData();
        }

        /// <summary>
        /// Is this an invalid owner of the objective ID?
        /// </summary>
        /// <returns></returns>
        public bool IsObjectiveIdOwnerInvalid()
        {
            //action ID is already null, so no checking of ownership is needed.
            if (actionID == null)
            {
                return false;
            }
            //get the ownership ID
            string ownerID = GetEditorOwnerID();

            //if null or empty, then the ID is invalid
            if (string.IsNullOrEmpty(ownerID))
            {
                return false;
            }

            //if id is the same, early out as this ObjectiveAction is the owner
            if (actionID.OwnerID == ownerID)
            {
                return false;
            }
            //invalid owner, clear data
            ClearIdData();

            //this was an invalid owner, return true
            return true;
        }

        /// <summary>
        /// Clear some ID data
        /// </summary>
        private void ClearIdData()
        {
            Undo.RecordObject(this, "Clear duplicate objective ID");

            //clear the fields associated with tracking and the ID
            actionID = null;
            isRecorded = false;

            EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// Get the object's global owner ID
        /// </summary>
        /// <returns>The ID that associated with the object</returns>
        public string GetEditorOwnerID()
        {
            //get the unique ID that denotes ownership of the action ID
            GlobalObjectId globalID = GlobalObjectId.GetGlobalObjectIdSlow(this);

            string id = globalID.ToString();

            //invalid ID, early out
            if (string.IsNullOrEmpty(id) || id.Contains("0000000000000000000"))
            {
                return null;
            }
            
            return id;
        }

        /// <summary>
        /// Create the SO action ID
        /// </summary>
        /// <param name="ownerID">The global ID that denotes which action has ownership
        ///                         of the to be created SO</param>
        public void CreateObjectiveID(string ownerID)
        {
            //safety check to make sure we still dont have an ID assigned
            if (actionID != null)
            {
                return;
            }
            //the folder the generated IDs will go into
            const string folder = "Assets/GeneratedObjectiveIDs";

            //if there is not a folder, create one
            if (!AssetDatabase.IsValidFolder(folder))
            {
                AssetDatabase.CreateFolder("Assets", "GeneratedObjectiveIDs");
            }

            //create an SO instance
            ObjectiveID newID = ScriptableObject.CreateInstance<ObjectiveID>();
            //set the ownership of this SO
            newID.SetOwner(ownerID);

            //ensure a clean name is generated
            string idName = gameObject.name.Replace("/", "_").Replace("\\", "_").Replace(":", "_");
            //the path the SO asset will live
            string path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{idName}_ObjectiveID.asset");

            //now create the SO asset
            AssetDatabase.CreateAsset(newID, path);

            Undo.RecordObject(this, "Create Objectived ID");

            //set the action ID to be this new SO
            actionID = newID;

            //set this and the SO assets as dirty
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(newID);

            AssetDatabase.SaveAssets();
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

        /// <summary>
        /// Set the quest ID this objective is part of
        /// </summary>
        /// <param name="id"></param>
        public void SetQuestID(QuestID id)
        {
            ActionQuestID = id;
            RegisterRecordedObjective();
        }

        /// <summary>
        /// Register this as a recorded objective that can be referenced later
        /// </summary>
        protected virtual void RegisterRecordedObjective()
        {
            if (!isRecorded)
            {
                return;
            }
            if (actionID == null)
            {
                return;
            }
            //add it to the backend database
            GoalManager.Instance.GoalTracker.AddRecordedObjective(ActionQuestID, actionID);
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

            //Mark the backend for the tracked objectives
            if (isRecorded && actionID != null)
            {
                GoalManager.Instance.GoalTracker.MarkObjectiveComplete(ActionQuestID, actionID);
            }

            SetState(ActionState.Inactive);
            OnActionCompleted.Invoke(this);
        }

        /// <summary>
        /// Is this the last in the list of a sequence based objective action?
        /// </summary>
        /// <returns></returns>
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