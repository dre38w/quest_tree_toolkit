/*
 * Description: Handles logic for interactable objects
 */
using Gameplay.UI;
using Service.Core;
using Service.Framework;
using Service.Framework.StatusSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.System
{
    public class InteractableObject : MonoBehaviour, IInteractable
    {
        [HideInInspector]
        public UnityEvent OnInteracted = new UnityEvent();

        private IInteractable interactable;

        /// <summary>
        /// The object that has a Trigger collider on it that initiates the interaction
        /// </summary>
        [SerializeField]
        private InteractableTrigger interactableObject;

        private PlayerInteractor interactor;
        private MainUI mainUI;

        /// <summary>
        /// Used to inform systems that the player interacted
        /// </summary>
        private bool didInteract;
        public bool DidInteract
        {
            get { return didInteract; }
            set { didInteract = value; }
        }

        private void Awake()
        {
            interactable = GetComponentInParent<IInteractable>();
            didInteract = false;
        }

        private void Start()
        {
            InitializeValues();
        }

        private void InitializeValues()
        {
            mainUI = ReferenceRegistry.Instance.MainUI;

            interactor = ReferenceRegistry.Instance.Player.GetComponent<PlayerInteractor>();
            interactor.OnInteracted.AddListener(OnPlayerInteracted);

            interactableObject.OnEnteredTriggerObject.AddListener(OnInteracting);
            interactableObject.OnExitedTriggerObject.AddListener(OnNotInteracting);
        }

        public void Interact(GameObject interactor)
        {
            //do not allow interaction
            if (!CanInteract(interactor))
            {
                return;
            }
        }

        private void OnInteracting()
        {
            mainUI.SetContextualUiVisible(true);

        }

        private void OnNotInteracting()
        {
            mainUI.SetContextualUiVisible(false);

        }

        /// <summary>
        /// Do stuff when the player pressed the interaction button
        /// </summary>
        private void OnPlayerInteracted()
        {
            if (interactor.CurrentInteractable != interactable)
            {
                return;
            }
            didInteract = true;
            mainUI.SetContextualUiVisible(false);
            OnInteracted.Invoke();
            StartCoroutine(ResetInteracted());
        }

        /// <summary>
        /// Reset next frame to allow other systems time to do logic
        /// </summary>
        /// <returns></returns>
        private IEnumerator ResetInteracted()
        {
            yield return null;
            didInteract = false;
        }

        /// <summary>
        /// This is the condition that allows this to be interactable
        /// </summary>
        /// <param name="interactor"></param>
        /// <returns>If the contextual button is active, 
        ///         then return true allowing us to interact</returns>
        public bool CanInteract(GameObject interactor)
        {
            return mainUI.ContextualButtonUI.activeSelf;
        }

        private void OnDestroy()
        {
            interactableObject.OnEnteredTriggerObject.RemoveListener(OnInteracting);
            interactableObject.OnExitedTriggerObject.RemoveListener(OnNotInteracting);
            interactor.OnInteracted.RemoveListener(OnPlayerInteracted);
            didInteract = false;
        }
    }
}