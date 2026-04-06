/*
 * Description: Example Action class that increases the player's level by one
 */

using Gameplay.System.Player;
using Service.Framework.Goals;

namespace Gameplay.System.Actions
{
    public class LevelUpPlayerActionExample : ObjectiveAction
    {
        public override void InitializeAction()
        {
            ReferenceRegistry.Instance.Player.GetComponent<PlayerStatExample>().UpdatePlayerLevel(1);
            SetComplete();
        }
    }
}