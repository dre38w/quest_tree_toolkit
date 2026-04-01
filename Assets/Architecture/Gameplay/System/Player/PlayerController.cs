/*
 * Description: Basic player movement
 */

using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.System.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 10f;
        private float gravity = -9.81f;
        [SerializeField]
        private float terminalVelocity = -20f;
        private float yVelocity;

        private Vector2 moveInput;
        private CharacterController characterController;

        private bool hasControl;
        public bool HasControl
        {
            get { return hasControl; }
            set { hasControl = value; }
        }

        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
        }

        private void Update()
        {
            if (characterController.isGrounded && yVelocity < 0)
            {
                yVelocity = -2;
            }

            Vector3 movePlayer = transform.right * moveInput.x + transform.forward * moveInput.y;

            yVelocity += gravity * Time.deltaTime;
            yVelocity = Mathf.Max(yVelocity, terminalVelocity);

            Vector3 velocity = movePlayer * moveSpeed;
            velocity.y = yVelocity;

            if (!hasControl)
            {
                return;
            }

            characterController.Move(velocity * Time.deltaTime);
        }

        public void GiveControl()
        {
            hasControl = true;
        }

        public void RemoveControl()
        {
            hasControl = false;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }
    }
}