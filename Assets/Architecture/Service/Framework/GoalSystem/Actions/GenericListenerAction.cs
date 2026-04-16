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