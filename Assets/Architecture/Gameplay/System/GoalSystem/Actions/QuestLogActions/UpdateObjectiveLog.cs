/*
 * Description: Updates the quest log
 */
using Service.Core;
using Service.Framework;
using Service.Framework.GoalManagement;
using Service.Framework.Goals;
using UnityEngine;

namespace Gameplay.System.Actions
{
    [Submenu("Quest Log Management/Update Objective")]
    public class UpdateObjectiveLog : ObjectiveAction
    {       
        /// <summary>
        /// The text that will display on the UI
        /// </summary>
        [Tooltip("Type in the text you want to display in the UI.")]
        [TextAreaAttribute(3, 10)]
        [SerializeField]
        private string textBox;
        [SerializeField]
        private bool isSubObjective;
        [SerializeField]
        private UpdateObjectiveLog parentObjective;

        public string CreatedObjectiveID { get; private set; }

        public override void InitializeAction()
        {
            //if the objective is the same, complete and ignore any updating
            if (!string.IsNullOrEmpty(CreatedObjectiveID))
            {
                ObjectiveData existingObjective = GoalManager.Instance.GoalTracker.GetObjective(ActionQuestID, CreatedObjectiveID);

                if (existingObjective != null)
                {
                    SetComplete();
                    return;
                }
            }

            //set the parent id for the new objective
            string parentID = null;

            if (isSubObjective && parentObjective != null)
            {
                parentID = parentObjective.CreatedObjectiveID;
            }

            //now add and create the new objective in the database
            ObjectiveData data = GoalManager.Instance.GoalTracker.AddObjective(ActionQuestID, textBox, isSubObjective, parentID);
            
            if (data == null)
            {
                return;
            }

            //we created the new ID when adding to the database, so reference it
            CreatedObjectiveID = data.ID;

            SetComplete();
        }
    }
}