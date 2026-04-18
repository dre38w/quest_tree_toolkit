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
            string parentID = null;

            if (isSubObjective && parentObjective != null)
            {
                parentID = parentObjective.CreatedObjectiveID;
            }

            ObjectiveData data = GoalManager.Instance.GoalTracker.AddObjective(ActionQuestID, textBox, isSubObjective, parentID);
            CreatedObjectiveID = data.ID;

            //GoalManager.Instance.GoalTracker.NotifyObjectiveChanges(ActionQuestID);
            SetComplete();
        }
    }
}