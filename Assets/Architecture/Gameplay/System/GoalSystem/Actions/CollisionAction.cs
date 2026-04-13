/*
 * Description: Action that completes when collision was detected
 */
using Service.Core;
using Service.Framework.Goals;
using UnityEngine;

namespace Gameplay.System.Actions
{
    [Submenu("Logic/Collision Detection")]
    public class CollisionAction : ObjectiveAction
    {
        [Tooltip("The object(s) the player is colliding with.")]
        [SerializeField]
        private CollisionActionComponent[] collisionActionComponent;

        private void Start()
        {
            if (collisionActionComponent.Length == 0)
            {
                Debug.LogWarning("collisionActionComponent list is empty.  Please populate this in the Inspector.", gameObject);
            }
            //use an array in the event we want to be able to collide with multiple objects
            for (int i = 0; i < collisionActionComponent.Length; i++)
            {
                collisionActionComponent[i].OnCollided.AddListener(OnCollided);
            }
        }

        /// <summary>
        /// Called when we first collided with any one of the specified collision objects
        /// </summary>
        private void OnCollided()
        {
            if (State == ActionState.Inactive)
            {
                return;
            }
            SetComplete();
            ResetValues();
        }

        public override void ResetValues()
        {
            for (int i = 0; i < collisionActionComponent.Length; i++)
            {
                collisionActionComponent[i].OnCollided.RemoveListener(OnCollided);
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < collisionActionComponent.Length; i++)
            {
                collisionActionComponent[i].OnCollided.RemoveListener(OnCollided);
            }
        }
    }
}