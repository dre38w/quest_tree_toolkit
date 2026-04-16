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