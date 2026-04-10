/*
 * Description: Holds references to objects to better organize when some objects need to be referenced many times, dynamically, etc. 
 *              This should be tailored to your game.
 */

using Gameplay.System.Player;
using Gameplay.UI;
using UnityEngine;

namespace Gameplay.System
{
    public class ReferenceRegistry : MonoBehaviour
    {
        public static ReferenceRegistry Instance;

        private UIStateHandler uiStateHandler;
        public UIStateHandler UiStateHandler
        {
            get { return uiStateHandler; }
            set { uiStateHandler = value; }
        }

        /// <summary>
        /// PlayerController is the main component on the player object and thus a reasonable point of reference
        /// </summary>
        private PlayerController player;
        public PlayerController Player
        {
            get { return player; }
            set { player = value; }
        }

        /// <summary>
        /// MainUI is the main component on the main UI object
        /// </summary>
        private MainUI mainUI;
        public MainUI MainUI
        {
            get { return mainUI; }
            set { mainUI = value; }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
            player = FindFirstObjectByType<PlayerController>();
            mainUI = FindFirstObjectByType<MainUI>();
            uiStateHandler = new UIStateHandler();
        }
    }
}