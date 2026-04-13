/*
 * Description: Used to set the float value on the blackboard
 */
using Service.Core;
using Service.Framework.GoalManagement;
using UnityEngine;

namespace Service.Framework.Goals
{
    [Submenu("Data/Blackboard/Set Float")]
    public class SetBlackboardFloatAction : ObjectiveAction
    {
        [Tooltip("The key that is stored in the Blackboard." +
                "Important to make sure the text typed in here is the exact same as it is in the Blackboard's database.")]
        [SerializeField]
        private string key;

        [SerializeField]
        private float floatValue;

        public override void InitializeAction()
        {
            GoalManager.Instance.BlackBoard.SetFloatValue(key, floatValue);
            SetComplete();
        }
    }
}