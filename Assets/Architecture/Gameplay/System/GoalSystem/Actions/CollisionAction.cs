/*
 * Description: Action that completes when collision was detected
 */
using Service.Framework.Goals;
using UnityEngine;

namespace Gameplay.System.Actions
{
    public class CollisionAction : ObjectiveAction
    {
        [Tooltip("The object the player is colliding with.")]
        [SerializeField]
        private CollisionActionComponent[] collisionActionComponent;

        private void Start()
        {
            if (collisionActionComponent.Length == 0)
            {
                Debug.LogWarning("collisionActionComponent list is empty.  Please populate this in the Inspector.", gameObject);
            }
            for (int i = 0; i < collisionActionComponent.Length; i++)
            {
                collisionActionComponent[i].OnCollided.AddListener(OnCollided);
            }
        }

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