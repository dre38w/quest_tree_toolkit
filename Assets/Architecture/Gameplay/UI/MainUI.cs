/*
 * Description: Handles main UI logic
 */

using Gameplay.System;
using Service.Core;
using Service.Framework;
using Service.Framework.Goals;
using Service.Framework.StatusSystem;
using System.Collections;
using UnityEngine;

namespace Gameplay.UI
{
    public class MainUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject contextualButtonUi;
        public GameObject ContextualButtonUI
        {
            get { return contextualButtonUi; }
            set { contextualButtonUi = value; }
        }

        public void SetContextualUiVisible(bool state)
        {
            contextualButtonUi.SetActive(state);
        }
    }
}