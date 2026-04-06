/*
 * Description: Interface used for interactable objects 
 */

using UnityEngine;

namespace Service.Core
{
    public interface IInteractable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interactor"></param>
        /// <returns></returns>
        bool CanInteract(GameObject interactor);
        /// <summary>
        /// When the player interacts via input
        /// </summary>
        /// <param name="interactor">In the event there is more than one player or other objects interacting, specify.</param>
        void Interact(GameObject interactor);
    }
}