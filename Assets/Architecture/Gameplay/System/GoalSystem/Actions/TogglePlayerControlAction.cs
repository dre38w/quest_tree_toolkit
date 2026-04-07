/*
 * Description: Toggles the player having control of the character.  
 *          Useful for cutscenes, scene transitions, dialog interactions, etc.
 */
using Service.Framework.Goals;
using System.Collections;
using UnityEngine;

namespace Gameplay.System.Actions
{
    public class TogglePlayerControlAction : ObjectiveAction
    {
        [SerializeField]
        private bool playerHasControl;

        public override void InitializeAction()
        {
            StartCoroutine(SetPlayerControl());
        }

        /// <summary>
        /// Run next frame in the event the managers still need to finish their startup logic
        /// </summary>
        /// <returns></returns>
        private IEnumerator SetPlayerControl()
        {
            yield return null;
            ReferenceRegistry.Instance.Player.SetControl(playerHasControl);
            SetComplete();
        }
    }
}