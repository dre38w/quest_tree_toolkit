/*
 * Description: Invoke a generic message via external scripts.
 * Useful if another gameplay element outside of the quest tree goal system needs to trigger an objective action or a goal to do something
 */

using UnityEngine;
using UnityEngine.Events;

namespace Service.Framework
{
    public class GenericQuestTreeMessenger : MonoBehaviour
    {
        public UnityEvent OnQuestTreeActionTriggered = new UnityEvent();

        public void OnTriggerAction()
        {
            OnQuestTreeActionTriggered.Invoke();
        }
    }
}