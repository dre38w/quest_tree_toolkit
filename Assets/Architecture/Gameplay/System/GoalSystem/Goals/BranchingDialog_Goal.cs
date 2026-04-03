/*
 * Description: Turns on or off the dialog box for dialog based goals 
 */

using Service.Framework.Goals;
using UnityEngine;

namespace Gameplay.System.Goals
{
    public class BranchingDialog_Goal : Goal
    {
        [SerializeField]
        private GameObject dialogBox;

        public override void InitializeGoal()
        {
            base.InitializeGoal();
            dialogBox.SetActive(true);
        }

        public override void SetComplete()
        {
            base.SetComplete();
            dialogBox.SetActive(false);
        }
    }
}