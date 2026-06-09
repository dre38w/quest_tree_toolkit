/*
 * Description: Handles the logic and conditions for the race mission
 */
using Service.Framework;
using System.Collections;
using UnityEngine;
using Gameplay.UI;

namespace Gameplay.System
{
    public class RaceQuest : MonoBehaviour
    {
        [SerializeField]
        private GenericQuestTreeMessenger questFailedMessenger;

        [SerializeField]
        private float raceDuration = 15f;
        private float currentRaceTime;

        [SerializeField]
        private RaceTimerUI raceTimerUI;

        private Coroutine raceTimerCoroutine;

        /// <summary>
        /// Perform the timer count
        /// </summary>
        /// <returns></returns>
        private IEnumerator RaceTimer()
        {
            currentRaceTime = raceDuration;
            while (currentRaceTime > 0)
            {
                currentRaceTime -= Time.deltaTime;
                raceTimerUI.DisplayTimer(Mathf.Abs(currentRaceTime));
                yield return null;
            }
            raceTimerUI.DisplayTimer(0);
            FailedRace();
            yield return null;
        }

        /// <summary>
        /// Called by external systems to start the race timer
        /// </summary>
        public void StartRace()
        {
            if (raceTimerCoroutine == null)
            {
                raceTimerCoroutine = StartCoroutine(RaceTimer());
            }
        }

        /// <summary>
        /// Notifies an Action in the goal tree that the race has failed
        /// </summary>
        private void FailedRace()
        {
            questFailedMessenger.OnTriggerAction();
            StopTimer();
        }

        public void StopTimer()
        {
            if (raceTimerCoroutine != null)
            {
                StopCoroutine(raceTimerCoroutine);
                raceTimerCoroutine = null;
            }
        }
    }
}