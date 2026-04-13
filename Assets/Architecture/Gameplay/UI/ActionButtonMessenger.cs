/*
 * Description: Invokes a message when a button is pressed.  Can be used for keyboard/controller input and UI buttons
 */
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.UI
{
    public class ActionButtonMessenger : MonoBehaviour
    {
        [HideInInspector]
        public UnityEvent OnActionButtonPressed = new UnityEvent();

        /// <summary>
        /// What the dialog action listens to.
        /// Call this method via whatever input system you are using.  
        /// OnClick for ui button, or via unity's input system 
        /// </summary>
        public void PressButton()
        {
            OnActionButtonPressed.Invoke();
        }
    }
}