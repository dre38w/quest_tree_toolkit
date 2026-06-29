/*
 * Description: Listens for when the GenericQuestTreeMessenger invokes its message
 */
using UnityEngine;

namespace Service.Framework.Goals
{
    public class GenericListenerAction : ObjectiveAction
    {
        [Tooltip("The object that will trigger this ObjectiveAction")]
        [SerializeField]
        private GenericQuestTreeMessenger genericMessenger;

        public override void InitializeAction()
        {
            //an external system invoked the message for this objective action to start its logic
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