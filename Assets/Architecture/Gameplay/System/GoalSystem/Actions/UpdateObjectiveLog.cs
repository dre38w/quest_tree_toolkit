using Service.Framework.GoalManagement;
using Service.Framework.Goals;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.System.Actions
{
    public class UpdateObjectiveLog : ObjectiveAction
    {       
        /// <summary>
        /// The text that will display on the UI
        /// </summary>
        [Tooltip("Type in the text you want to display in the UI.")]
        [TextAreaAttribute(3, 10)]
        [SerializeField]
        private string textBox;

        public override void InitializeAction()
        {
            GoalManager.Instance.GoalTracker.AddObjective(ActionGoalID, textBox);          
            SetComplete();
        }
    }
}