/*
 * Description: Checks whether or not the player has pressed the interaction button 
 */

using Service.Framework.Goals;
using System;

namespace Gameplay.System.Requirements
{
    [Serializable]
    public class GoalRequirement_Interact : GoalRequirement
    {
        public InteractableObject interactable;

        /// <summary>
        /// Return the value of the interaction flag
        /// </summary>
        /// <param name="goalToCheck"></param>
        /// <returns>The flag we are checking.  When the player presses the interact button, it returns true.</returns>
        public override bool IsRequirementMet(Goal goalToCheck)
        {
            return interactable.DidInteract;
        }
    }
}