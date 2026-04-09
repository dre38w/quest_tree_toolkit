/*
 * Description: Handles main UI logic
 */

using Gameplay.System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.UI
{
    public enum UIState
    {
        Default,
        Gameplay,
        Pause,
    }
    
    public class MainUI : MonoBehaviour
    {
        [SerializeField]
        private InputActionAsset inputActions;
        protected InputAction MenuButtonAction { get; set; }

        [SerializeField]
        private GameObject menu;

        [SerializeField]
        private GameObject contextualButtonUi;
        public GameObject ContextualButtonUI
        {
            get { return contextualButtonUi; }
            set { contextualButtonUi = value; }
        }

        public UIState uiState;

        private void Awake()
        {
            MenuButtonAction = inputActions.FindAction("Menu");
            MenuButtonAction.started += OnToggleMenu;
        }

        protected virtual void OnToggleMenu(InputAction.CallbackContext context)
        {
            if (uiState != UIState.Default)
            {
                return;
            }
            if (context.started)
            {               
                //set the values based on the state of the menu
                menu.SetActive(!menu.activeSelf);
                ReferenceRegistry.Instance.Player.ToggleCursorLock(!menu.activeSelf);
                ReferenceRegistry.Instance.Player.SetControl(!menu.activeSelf);
            }
        }

        public void SetUIState(UIState state)
        {
            if (uiState == state)
            {
                return;
            }
            uiState = state;
        }

        /// <summary>
        /// Toggles on or off the contextual action button.
        /// </summary>
        /// <param name="state"></param>
        public void SetContextualUiVisible(bool state)
        {
            contextualButtonUi.SetActive(state);
        }

        private void OnDestroy()
        {
            MenuButtonAction.started -= OnToggleMenu;

        }
    }
}