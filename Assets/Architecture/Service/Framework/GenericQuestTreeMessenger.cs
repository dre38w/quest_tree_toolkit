/*
 * Description:  Generic messenger that gets invoked via the InvokeGenericMessageAction.
 *          This is useful for triggering any other logic outside of the Quest Tree system.
 *          Such as animations, checkpoints, etc.
 *          
 *  *** NOTE ***
 *      The object that you want the InvokeGenericMessageAction to trigger logic on
 *      must reference this object and have a listener added waiting for the invoked message.
 *      The InvokeGenericMessageAction must have the same reference
 */
using UnityEngine;
using UnityEngine.Events;

namespace Service.Framework
{
    
    public class GenericQuestTreeMessenger : MonoBehaviour
    {
        public UnityEvent OnQuestTreeMessageTriggered = new UnityEvent();
    }
}