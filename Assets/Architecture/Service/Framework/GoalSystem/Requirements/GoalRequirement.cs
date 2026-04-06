/*
 * Description: Base class for all requirements.
 */
using System;

namespace Service.Framework.Goals
{
    [Serializable]
    public abstract class GoalRequirement
    {
        public abstract bool IsRequirementMet(Goal goalToCheck);
        
    }
}