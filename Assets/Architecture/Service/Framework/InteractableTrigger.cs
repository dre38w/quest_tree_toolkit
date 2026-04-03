/*
 * Description:  Used to inform systems when the player enters a trigger
 */
using Service.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Service.Framework.StatusSystem
{
    public class InteractableTrigger : MonoBehaviour
    {
        private IInteractable interactable;

        [HideInInspector]
        public UnityEvent OnEnteredTriggerObject = new UnityEvent();
        [HideInInspector]
        public UnityEvent OnExitedTriggerObject = new UnityEvent();

        private void Awake()
        {
            interactable = GetComponentInChildren<IInteractable>();
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerInteractor interactor = other.GetComponentInParent<PlayerInteractor>();
            if (interactor == null)
            {
                return;
            }
            interactor.SetCurrentInteractable(interactable);
            OnEnteredTriggerObject.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            PlayerInteractor interactor = other.GetComponentInParent<PlayerInteractor>();
            if (interactor == null)
            {
                return;
            }
            interactor.ClearCurrentInteractable(interactable);
            OnExitedTriggerObject.Invoke();
        }
    }
}