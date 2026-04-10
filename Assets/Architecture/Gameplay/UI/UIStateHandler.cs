/*
 * Description: Controls the state the UI is in
 */
using UnityEngine.Events;

namespace Gameplay.UI
{
    public class UIStateHandler
    {
        public enum UIState
        {
            Default,
            Gameplay,
            Pause,
        }
        public UIState uiState;

        public UnityEvent<UIState> OnUiStateChanged = new UnityEvent<UIState>();

        /// <summary>
        /// Handle state changing here to allow for more generic use
        /// </summary>
        /// <param name="state"></param>
        public void SetUIState(UIState state)
        {
            if (uiState == state)
            {
                return;
            }
            uiState = state;

            OnUiStateChanged.Invoke(uiState);
        }
    }
}