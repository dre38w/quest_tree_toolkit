/*
 * Description: Used to set the vector3 value on the blackboard
 */
using Service.Core;
using Service.Framework.GoalManagement;
using UnityEngine;

namespace Service.Framework.Goals
{
    [Submenu("Data/Blackboard/Set Vector3")]
    public class SetBlackboardVector3Action : ObjectiveAction
    {
        [Tooltip("The key that is stored in the Blackboard." +
               "Important to make sure the text typed in here is the exact same as it is in the Blackboard's database.")]
        [SerializeField]
        private string key;

        [SerializeField]
        private Vector3 vector3Value;

        public override void InitializeAction()
        {
            GoalManager.Instance.BlackBoard.SetVector3Value(key, vector3Value);
            SetComplete();
        }
    }
}