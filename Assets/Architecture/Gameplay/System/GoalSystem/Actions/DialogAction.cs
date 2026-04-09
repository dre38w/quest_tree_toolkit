/*
 * Description: Example script on a branching dialog mechanic
 */

using Gameplay.UI;
using Service.Framework.Goals;
using TMPro;
using UnityEngine;

namespace Gameplay.System.Actions
{
    public class DialogAction : ObjectiveAction
    {
        /// <summary>
        /// The text that will display on the UI
        /// </summary>
        [Tooltip("Type in the text you want to display in the UI dialog box.")]
        [TextAreaAttribute(3, 10)]
        [SerializeField]
        private string dialogBox;
        [SerializeField]
        private TMP_Text displayText;

        [Tooltip("The button that will advance the dialog.")]
        [SerializeField]
        private ButtonMessenger actionButton;

        [Tooltip("Is this requiring the player to input a response?")]
        [SerializeField]
        private bool isResponse = false;

        public override void InitializeAction()
        {
            //turn on the action button
            actionButton.gameObject.SetActive(true);
            //set the UI state so certain UIs don't cross
            ReferenceRegistry.Instance.MainUI.SetUIState(UIState.Gameplay);
            DisplayText();
            actionButton.OnButtonPressed.AddListener(OnContinueButtonPressed);
        }

        public override void ReinitializeAction()
        {
            base.ReinitializeAction();
            ResetValues();
        }

        /// <summary>
        /// Display the text that was entered in the dialogBox field
        /// </summary>
        private void DisplayText()
        {
            displayText.text = dialogBox;
        }

        private void OnContinueButtonPressed()
        {
            //if this is a response, branch the action.
            //This is because the multi-choice response text has a button overlayed on top of it.
            if (isResponse)
            {
                isBranching = true;
            }
            SetComplete();
        }

        public override void SetComplete()
        {
            base.SetComplete();
            ResetValues();
        }

        public override void ResetValues()
        {
            ReferenceRegistry.Instance.MainUI.SetUIState(UIState.Default);

            displayText.text = string.Empty;
            actionButton.OnButtonPressed.RemoveListener(OnContinueButtonPressed);
            actionButton.gameObject.SetActive(false);

        }
    }
}