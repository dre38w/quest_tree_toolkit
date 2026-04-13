/*
 * Description: Adds a delay before completing the action.  Useful for creating a time buffer between other actions
 */
using Service.Core;
using System.Collections;
using UnityEngine;

namespace Service.Framework.Goals
{
    [Submenu("Logic/Delay")]
    public class DelayCompleteAction : ObjectiveAction
    {
        [SerializeField]
        private float delayDuration;

        public override void InitializeAction()
        {
            StartCoroutine(DelayAction());
        }

        private IEnumerator DelayAction()
        {
            yield return new WaitForSeconds(delayDuration);
            SetComplete();
        }
    }
}