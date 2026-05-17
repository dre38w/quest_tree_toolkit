using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.System
{
    public class Checkpoint : MonoBehaviour
    {
        [HideInInspector]
        public UnityEvent OnReachedCheckpoint = new UnityEvent();

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagData.PLAYER_TAG))
            {
                OnReachedCheckpoint.Invoke();
            }
    }
    }
}