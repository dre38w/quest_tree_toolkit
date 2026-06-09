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

        [Tooltip("Do we want to be able to collide with the objects more than once?")]
        [SerializeField]
        private bool multiCollision = false;

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
                //activate the next collision object in the list
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

            //require all objects in the list to be collided with
            if (requireAllCollisions)
            {
                //if we already removed this object, early out.
                //if was not removed yet, remove it.
                //this allows us to process a collision event once,
                //since the goal is to require the player to collide with all in the list, we only care if they did so once
                if (!remainingCollisionObjects.Remove(collidedObject))
                {
                    return;
                }

                //still have objects to collide with, early out
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
            //don't remove the listeners if we want to allow multi collision detection 
            if (multiCollision)
            {
                return;
            }
            //remove the listeners to avoid collisions from triggering 
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