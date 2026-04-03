using Service.Framework.Goals;
using System;

[Serializable]
public class GoalRequirement_Interact : GoalRequirement
{
    public InteractableObject interactableHandler;

    public override bool IsRequirementMet(Goal goalToCheck)
    {
        return interactableHandler.didInteract;
    }
}
