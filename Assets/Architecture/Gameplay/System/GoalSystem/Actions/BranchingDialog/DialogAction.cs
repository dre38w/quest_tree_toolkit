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
        [TextAreaAttribute(3, 10)]
        [SerializeField]
        private string dialogBox;
        [SerializeField]
        private TMP_Text displayText;

        [SerializeField]
        private ButtonMessenger actionButton;

        [Tooltip("Is this player character dialog?")]
        [SerializeField]
        private bool isResponse = false;

        public override void InitializeAction()
        {
            actionButton.gameObject.SetActive(true);
            DisplayText();
            actionButton.OnButtonPressed.AddListener(OnContinueButtonPressed);
        }

        public override void ReinitializeAction()
        {
            base.ReinitializeAction();
            ResetValues();
        }

        private void DisplayText()
        {
            displayText.text = dialogBox;
        }

        private void OnContinueButtonPressed()
        {
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
            displayText.text = string.Empty;
            actionButton.OnButtonPressed.RemoveListener(OnContinueButtonPressed);
            actionButton.gameObject.SetActive(false);

        }
    }
}