/*
 * Description: Example script that handles player stat data
 */

using Service.Framework.GoalManagement;
using UnityEngine;

namespace Gameplay.System.Player
{
    public class PlayerStatExample : MonoBehaviour
    {
        private int level;

        /// <summary>
        /// Update the player's level by an amount
        /// </summary>
        /// <param name="value">Passing a negative value decreases the level.
        ///                     A positive value increases it.</param>
        public void UpdatePlayerLevel(int value)
        {
            level += value;
            //pass the unique name of the variable you want to store
            GoalManager.Instance.BlackBoard.SetIntValue("Player_" + nameof(level), level);
        }
    }
}