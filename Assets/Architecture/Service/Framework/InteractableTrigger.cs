/*
 * Description:  Used to inform systems when the player enters a trigger that initiates an interaction with interactable objects
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
            //set the interactable we want to interact with
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
            //When leaving the trigger, clear the interactable since we are no longer interacting
            interactor.ClearCurrentInteractable(interactable);
            OnExitedTriggerObject.Invoke();
        }
    }
}