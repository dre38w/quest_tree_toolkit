/*
 * Description: Completes when the player presses the interact button.
 */
using Service.Core;
using Service.Framework.Goals;
using UnityEngine;

namespace Gameplay.System.Actions
{
    [Submenu("Player/Player Interact")]
    public class PlayerInteractAction : ObjectiveAction
    {
        //the object that will trigger the interaction button
        [SerializeField]
        private InteractableObject interactableObject;

        public override void InitializeAction()
        {
            //when the player performed the action that triggers the interaction
            interactableObject.OnInteracted.AddListener(OnPlayerInteracted);
        }

        public void OnPlayerInteracted()
        {
            SetComplete();
            ResetValues();
        }

        public override void ResetValues()
        {
            interactableObject.OnInteracted.RemoveListener(OnPlayerInteracted);

        }
    }
}