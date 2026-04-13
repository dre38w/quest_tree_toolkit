/*
 * Description: Invokes a generic message that other systems can listen for to do logic not included in the quest tree.
 *          This could be playing an animation, a checkpoint, etc.
 * 
 *  *** NOTE ***
 *      The object that you want this action to trigger logic on
 *      must reference the same GenericQuestTreeMessenger and have a listener added waiting for the invoked message
 */
using UnityEngine;

namespace Service.Framework.Goals
{
    public class InvokeGenericMessageAction : ObjectiveAction
    {
        [Header("Reference the object you want this action to trigger.")]
        [SerializeField]
        private GenericQuestTreeMessenger[] genericMessengers;

        public override void InitializeAction()
        {
            //in the event we want to trigger multiple, iterate over them
            for (int i = 0; i < genericMessengers.Length; i++)
            {
                genericMessengers[i].OnQuestTreeMessageTriggered.Invoke();
            }
            SetComplete();
        }
    }
}