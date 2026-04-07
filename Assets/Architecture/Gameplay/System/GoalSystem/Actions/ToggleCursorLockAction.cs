/*
 * Description: Toggles the player's cursor's lock state.  Useful for dialog boxes, minigames, QTEs, etc.
 */
using Service.Framework.Goals;
using UnityEngine;

namespace Gameplay.System.Actions
{
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