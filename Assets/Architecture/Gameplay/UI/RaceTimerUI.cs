/*
 * Description: Displays the timer to the UI
 */
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    public class RaceTimerUI : MonoBehaviour
    {
        [SerializeField]
        private Text timerText;

        public void DisplayTimer(float displayTime)
        {
            timerText.text = displayTime.ToString("F2");
        }
    }
}