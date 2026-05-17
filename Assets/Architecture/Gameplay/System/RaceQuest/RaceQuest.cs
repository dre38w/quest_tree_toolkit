using Service.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.System
{
    public class RaceQuest : MonoBehaviour
    {
        //[SerializeField]
        //private GenericQuestTreeMessenger questCompleteMessenger;
        [SerializeField]
        private GenericQuestTreeMessenger questFailedMessenger;

        [SerializeField]
        private float raceDuration = 15f;
        private float currentRaceTime;

        //[SerializeField]
        //private List<Checkpoint> checkpoints = new List<Checkpoint>();

        private RaceTimerUI raceTimerUI;

        //private int currentCheckpoint = 0;

        private Coroutine raceTimerCoroutine;

        private void Start()
        {
            raceTimerUI = ReferenceRegistry.Instance.MainUI.GetComponent<RaceTimerUI>();

            //StartCoroutine(Initialize());
        }

        //private IEnumerator Initialize()
        //{
        //    yield return null;
        //    for (int i = 0; i < checkpoints.Count; i++)
        //    {
        //        checkpoints[i].gameObject.SetActive(false);
        //    }
        //}

        private IEnumerator RaceTimer()
        {
            currentRaceTime = raceDuration;
            while (currentRaceTime > 0)
            {
                currentRaceTime -= Time.deltaTime;
                raceTimerUI.DisplayTimer(currentRaceTime);
                yield return null;
            }
            raceTimerUI.DisplayTimer(0);
            FailedRace();
            yield return null;
        }

        public void StartRace()
        {
            //checkpoints[0].gameObject.SetActive(true);

            if (raceTimerCoroutine == null)
            {
                raceTimerCoroutine = StartCoroutine(RaceTimer());
            }
        }

        private void FailedRace()
        {
            questFailedMessenger.OnTriggerAction();

            StopTimer();

            //for (int i = 0; i < checkpoints.Count; i++)
            //{
            //    checkpoints[i].gameObject.SetActive(false);
            //}
            //startRace.SetActive(true);
            //currentCheckpoint = 0;
        }

        public void StopTimer()
        {
            if (raceTimerCoroutine != null)
            {
                StopCoroutine(raceTimerCoroutine);
                raceTimerCoroutine = null;
            }
        }

        private void OnDestroy()
        {
            //for (int i = 0; i < checkpoints.Count; i++)
            //{
            //    //checkpoints[i].OnReachedCheckpoint.RemoveListener(ReachedCheckpoint);
            //}
        }
    }
}