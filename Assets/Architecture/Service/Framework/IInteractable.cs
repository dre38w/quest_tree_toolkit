/*
 * Description: Interface used for interactable objects 
 */

using UnityEngine;

namespace Service.Core
{
    public interface IInteractable
    {
        bool CanInteract(GameObject interactor);
        void Interact(GameObject interactor);
    }
}