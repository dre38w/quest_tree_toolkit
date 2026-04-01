/*
 * Description: Handles camera look logic 
 */

using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.System.Player
{
    public class CameraLookController : MonoBehaviour
    {
        [SerializeField]
        private float mouseSensitivity = 150f;
        [SerializeField]
        private float yClamp = 80f;

        private float xRotation = 0f;
        [SerializeField]
        private Transform player;

        private Vector2 lookInput;

        private PlayerController playerController;

        private void Start()
        {
            playerController = player.GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (!playerController.HasControl)
            {
                return;
            }
            float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
            float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -yClamp, yClamp);

            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            player.Rotate(Vector3.up * mouseX);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();
        }
    }
}