/*
 * Description: Toggles the player's cursor's lock state.  Useful for dialog boxes, minigames, QTEs, etc.
 */
using Service.Core;
using Service.Framework.Goals;
using UnityEngine;

namespace Gameplay.System.Actions
{
    [Submenu("Player/Toggle Cursor Lock")]
    public class ToggleCursorLockAction : ObjectiveAction
    {
        [SerializeField]
        private bool isCursorLocked;

        public override void InitializeAction()
        {
            if (isCursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            SetComplete();
        }
    }
}