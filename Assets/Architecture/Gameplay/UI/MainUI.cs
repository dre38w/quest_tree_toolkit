/*
 * Description: Handles main UI logic.  This is a simple example and should be tailored to your game
 */

using Gameplay.System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.UI
{    
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

        private UIStateHandler stateHandler;

        private void Awake()
        {
            MenuButtonAction = inputActions.FindAction("Menu");
            MenuButtonAction.started += OnToggleMenu;
        }

        private void Start()
        {
            stateHandler = ReferenceRegistry.Instance.UiStateHandler;
            stateHandler.OnUiStateChanged.AddListener(OnStateChanged);
        }

        protected virtual void OnToggleMenu(InputAction.CallbackContext context)
        {
            //consider this menu part of the "default" ui state layer.
            //this is to prevent other UI logic from interrupting or conflicting
            if (stateHandler.uiState != UIStateHandler.UIState.Default)
            {
                menu.SetActive(false);
                return;
            }
            if (context.started)
            {               
                //set the values based on the opposite state of the menu
                menu.SetActive(!menu.activeSelf);
                ReferenceRegistry.Instance.Player.ToggleCursorLock(!menu.activeSelf);
                ReferenceRegistry.Instance.Player.SetControl(!menu.activeSelf);
            }
        }

        private void OnStateChanged(UIStateHandler.UIState state)
        {
            //set default values when entering a new state
            if (stateHandler.uiState == UIStateHandler.UIState.Default)
            {

            }
            else if (stateHandler.uiState == UIStateHandler.UIState.Gameplay)
            {
                menu.SetActive(false);
            }
            else if (stateHandler.uiState == UIStateHandler.UIState.Pause)
            {
                menu.SetActive(false);
            }
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
            stateHandler.OnUiStateChanged.RemoveListener(OnStateChanged);

        }
    }
}