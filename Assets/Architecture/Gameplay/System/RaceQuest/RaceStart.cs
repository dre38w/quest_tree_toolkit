using UnityEngine;

namespace Gameplay.System
{
    public class RaceStart : MonoBehaviour
    {
        [SerializeField]
        private RaceQuest raceQuest;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagData.PLAYER_TAG))
            {
                raceQuest.StartRace();
            }
        }
    }
}