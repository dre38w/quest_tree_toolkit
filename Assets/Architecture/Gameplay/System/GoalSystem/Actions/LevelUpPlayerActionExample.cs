/*
 * Description: Example Action class that increases the player's level by one
 */

using Gameplay.System.Player;
using Service.Core;
using Service.Framework.Goals;

namespace Gameplay.System.Actions
{
    [Submenu("Other/Debug/Player Level Up")]
    public class LevelUpPlayerActionExample : ObjectiveAction
    {
        public override void InitializeAction()
        {
            ReferenceRegistry.Instance.Player.GetComponent<PlayerStatExample>().UpdatePlayerLevel(1);
            SetComplete();
        }
    }
}