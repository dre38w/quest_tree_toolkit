/*
 * Description: Invokes a message when a button is pressed
 */
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.UI
{
    public class ButtonMessenger : MonoBehaviour
    {
        [HideInInspector]
        public UnityEvent OnButtonPressed = new UnityEvent();

        public void PressButton()
        {
            OnButtonPressed.Invoke();
        }
    }
}