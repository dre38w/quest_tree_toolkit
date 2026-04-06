/*
 * Description: Action that turns an object on or off
 */
using UnityEngine;

namespace Service.Framework.Goals
{
    public class ToggleObjectActiveAction : ObjectiveAction
    {
        /// <summary>
        /// Allow multiple objects to be toggled
        /// </summary>
        [SerializeField]
        private GameObject[] objectsToToggle;

        [SerializeField]
        private bool setObjectActive;

        public override void InitializeAction()
        {
            for (int i = 0; i < objectsToToggle.Length; i++)
            {
                objectsToToggle[i].SetActive(setObjectActive);
            }
            SetComplete();
        }
    }
}