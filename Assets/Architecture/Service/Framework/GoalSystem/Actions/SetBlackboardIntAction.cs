/*
 * Description: Used to set the int value on the blackboard
 */
using Service.Core;
using Service.Framework.GoalManagement;
using UnityEngine;

namespace Service.Framework.Goals
{
    [Submenu("Data/Blackboard/Set Integer")]
    public class SetBlackboardIntAction : ObjectiveAction
    {
        [Tooltip("The key that is stored in the Blackboard." +
                "Important to make sure the text typed in here is the exact same as it is in the Blackboard's database.")]
        [SerializeField]
        private string key;

        [SerializeField]
        private int intValue;

        public override void InitializeAction()
        {
            GoalManager.Instance.BlackBoard.SetIntValue(key, intValue);
            SetComplete();
        }
    }
}