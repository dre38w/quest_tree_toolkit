using System;

namespace Service.Framework.Goals
{
    [Serializable]
    public abstract class GoalRequirement
    {
        public abstract bool IsRequirementMet(Goal goalToCheck);
        
    }
}