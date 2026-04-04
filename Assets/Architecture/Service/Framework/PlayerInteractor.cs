/*
 * Description: Generic script that serves as the handshake for interactable objects
 */

using Service.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Service.Framework
{
    public class PlayerInteractor : MonoBehaviour
    {
        [HideInInspector]
        public UnityEvent OnInteracted = new UnityEvent();

        protected InputAction InteractAction { get; set; }

        public IInteractable CurrentInteractable { get; private set; }

        [SerializeField]
        private InputActionAsset inputActions;

        private void Awake()
        {
            InteractAction = inputActions.FindAction("Interact");
            InteractAction.started += OnInteract;
        }

        public void SetCurrentInteractable(IInteractable interactable)
        {
            CurrentInteractable = interactable;
        }

        public void ClearCurrentInteractable(IInteractable interactable)
        {
            if (CurrentInteractable == interactable)
            {
                CurrentInteractable = null;
            }
        }

        protected virtual void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (CurrentInteractable?.CanInteract(gameObject) == true)
                {
                    //pass this game object so systems know which object is being interacted with
                    CurrentInteractable.Interact(gameObject);
                    OnInteracted.Invoke();
                }
            }
        }

        private void OnDestroy()
        {
            InteractAction.started -= OnInteract;

        }
    }
}