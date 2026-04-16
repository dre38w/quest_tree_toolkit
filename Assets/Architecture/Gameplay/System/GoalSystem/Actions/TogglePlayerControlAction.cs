/*
 * Description: Handles toggleable player controls such as locked/unlocked cursor, giving/removing input control, etc.
 *          Useful for cutscenes, scene transitions, dialog interactions, etc.
 */
using Service.Core;
using Service.Framework.Goals;
using System.Collections;
using UnityEngine;

namespace Gameplay.System.Actions
{
    [Submenu("Player/Toggle Player Control")]
    public class TogglePlayerControlAction : ObjectiveAction
    {
        [SerializeField]
        private bool playerHasControl = true;
        [SerializeField]
        private bool isCursorLocked = true;

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
            ReferenceRegistry.Instance.Player.ToggleCursorLock(isCursorLocked);
            ReferenceRegistry.Instance.Player.SetControl(playerHasControl);
            SetComplete();
        }
    }
}