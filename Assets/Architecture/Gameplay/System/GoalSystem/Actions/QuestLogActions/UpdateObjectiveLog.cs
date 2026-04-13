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

        public string CreatedObjectiveID { get; private set; }

        public override void InitializeAction()
        {
            ObjectiveData data = GoalManager.Instance.GoalTracker.AddObjective(ActionQuestID, textBox);
            CreatedObjectiveID = data.ID;
            SetComplete();
        }
    }
}