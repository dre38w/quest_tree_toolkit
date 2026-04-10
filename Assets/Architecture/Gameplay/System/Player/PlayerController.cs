/*
 * Description: Basic player movement
 */

using System.Collections;
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

        /// <summary>
        /// Short delay before allowing player to have control of the character
        /// This will allow time for the game to load some things
        /// </summary>
        private float delayControlTime = 3f;

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
            StartCoroutine(DelayGiveControl());
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private IEnumerator DelayGiveControl()
        {
            yield return new WaitForSeconds(delayControlTime);
            hasControl = true;
        }
                
        public void ToggleCursorLock(bool lockState)
        {
            if (lockState)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        /// <summary>
        /// Used to give or remove control from the player
        /// </summary>
        /// <param name="state"></param>
        public void SetControl(bool state)
        {
            hasControl = state;
        }

        private void Update()
        {
            if (characterController.isGrounded && yVelocity < 0)
            {
                yVelocity = -2;
            }

            Vector3 movePlayer = transform.right * moveInput.x + transform.forward * moveInput.y;

            //continuously apply gravity
            yVelocity += gravity * Time.deltaTime;
            yVelocity = Mathf.Max(yVelocity, terminalVelocity);

            Vector3 velocity = movePlayer * moveSpeed;
            velocity.y = yVelocity;

            //do not allow input from the player if they don't have control
            if (!hasControl)
            {
                return;
            }

            characterController.Move(velocity * Time.deltaTime);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }
    }
}