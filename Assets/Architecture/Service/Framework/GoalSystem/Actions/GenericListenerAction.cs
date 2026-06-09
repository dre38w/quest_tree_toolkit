/*
 * Description: Listens for when the quest tree messenger invokes its message
 */
using UnityEngine;

namespace Service.Framework.Goals
{
    public class GenericListenerAction : ObjectiveAction
    {
        [SerializeField]
        private GenericQuestTreeMessenger genericMessenger;

        public override void InitializeAction()
        {
            genericMessenger.OnQuestTreeActionTriggered.AddListener(OnTriggerComplete);
        }

        private void OnTriggerComplete()
        {
            //don't react if in inactive state
            if (State == ActionState.Inactive)
            {
                return;
            }
            SetComplete();
            ResetValues();
        }

        public override void ResetValues()
        {
            genericMessenger.OnQuestTreeActionTriggered.RemoveListener(OnTriggerComplete);

        }
    }
}