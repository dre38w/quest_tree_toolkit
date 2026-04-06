/*
 * Description: Example class of detecting collision with the player.
 */
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.System.Actions
{
    public class CollisionActionComponent : MonoBehaviour
    {
        [HideInInspector]
        public UnityEvent OnCollided = new UnityEvent();

        [Tooltip("Is this a Trigger collider?")]
        [SerializeField]
        private bool isTrigger;
        [Tooltip("If true only Exit events are detected.  If false, only Enter events are detected.")]
        [SerializeField]
        private bool isExitEvent;

        /// <summary>
        /// Used if this object moves and collides with the player.
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            if (isTrigger || isExitEvent)
            {
                return;
            }

            if (collision.gameObject.CompareTag(TagData.PLAYER_TAG))
            {
                OnCollided.Invoke();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (isTrigger || !isExitEvent)
            {
                return;
            }

            if (collision.gameObject.CompareTag(TagData.PLAYER_TAG))
            {
                OnCollided.Invoke();
            }
        }

        /// <summary>
        /// Used if the player or this object collide with the player
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (!isTrigger || isExitEvent)
            {
                return;
            }

            if (other.gameObject.CompareTag(TagData.PLAYER_TAG))
            {
                OnCollided.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isTrigger || !isExitEvent)
            {
                return;
            }

            if (other.gameObject.CompareTag(TagData.PLAYER_TAG))
            {
                OnCollided.Invoke();
            }
        }
    }
}