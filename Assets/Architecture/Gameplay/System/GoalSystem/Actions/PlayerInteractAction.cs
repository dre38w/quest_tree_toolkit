/*
 * Description: Completes when the player presses the interact button.
 */
using Service.Framework.Goals;
using UnityEngine;

namespace Gameplay.System.Actions
{
    public class PlayerInteractAction : ObjectiveAction
    {
        [SerializeField]
        private InteractableObject interactableObject;

        public override void InitializeAction()
        {
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