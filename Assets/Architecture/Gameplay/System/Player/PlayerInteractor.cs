/*
 * Description: Generic script that serves as the handshake for interactable objects
 */

using Service.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Gameplay.System.Player
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

        /// <summary>
        /// Set the object we are interacting with
        /// </summary>
        /// <param name="interactable"></param>
        public void SetCurrentInteractable(IInteractable interactable)
        {
            CurrentInteractable = interactable;
        }

        /// <summary>
        /// Clear the interactable object
        /// </summary>
        /// <param name="interactable"></param>
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
                    //pass this game object in the event there is more than one player or
                    //other objects are interacting with the interactable objects
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