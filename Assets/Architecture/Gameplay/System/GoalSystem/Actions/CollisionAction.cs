/*
 * Description: Action that completes when collision was detected
 */
using Service.Core;
using Service.Framework.Goals;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.System.Actions
{
    [Submenu("Logic/Collision Detection")]
    public class CollisionAction : ObjectiveAction
    {
        [Tooltip("The object(s) the player is colliding with.")]
        [SerializeField]
        private CollisionActionComponent[] collisionActionComponent;

        [Header("Collision Settings")]

        [Tooltip("Should the player be required to collide with all objects in the list?")]
        [SerializeField]
        private bool requireAllCollisions = false;

        [Tooltip("Should the object we collide with be deactivated?")]
        [SerializeField]
        private bool deactivateCollisionObject = false;

        [Tooltip("Should the next object in the list be activated?")]
        [SerializeField]
        private bool activateNextCollisionObject = false;

        private HashSet<GameObject> remainingCollisionObjects = new HashSet<GameObject>();

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
                remainingCollisionObjects.Add(collisionActionComponent[i].gameObject);
            }
        }

        /// <summary>
        /// Called when we first collided with any one of the specified collision objects
        /// </summary>
        private void OnCollided(GameObject collidedObject)
        {
            if (State == ActionState.Inactive)
            {
                return;
            }

            if (deactivateCollisionObject)
            {
                collidedObject.SetActive(false);
            }

            if (activateNextCollisionObject)
            {
                
                for (int i = 0; i < collisionActionComponent.Length; i++)
                {
                    if (collisionActionComponent[i].gameObject == collidedObject)
                    {
                        int nextIndex = i + 1;

                        if (nextIndex < collisionActionComponent.Length)
                        {
                            collisionActionComponent[nextIndex].gameObject.SetActive(true);
                        }
                        break;
                    }
                }
            }

            if (requireAllCollisions)
            {
                if (!remainingCollisionObjects.Remove(collidedObject))
                {
                    return;
                }

                if (remainingCollisionObjects.Count > 0)
                {
                    return;
                }
            }

            SetComplete();
            ResetValues();
        }

        public override void ResetValues()
        {
            for (int i = 0; i < collisionActionComponent.Length; i++)
            {
                //collisionActionComponent[i].OnCollided.RemoveListener(OnCollided);
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