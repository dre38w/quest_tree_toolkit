/*
 * Description: Used to inform user that a serialized reference was lost.
 *              NOTE: This class should remain in the current namespace to avoid foundational errors.
 */
using Service.Framework.Goals;
using System;
using UnityEngine;

namespace Service.Core
{
    [Serializable]
    public sealed class DefaultRequirement : GoalRequirement
    {
        public override bool IsRequirementMet(Goal goalToCheck)
        {
#if UNITY_EDITOR
            Debug.LogError($"Repaired a missing serialized reference and replaced with a default goal requirement. " +
                " Please review the goal's requirements list.", goalToCheck);
#endif
            return false;
        }
    }
}