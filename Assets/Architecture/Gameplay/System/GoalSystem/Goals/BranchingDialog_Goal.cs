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

        public override void GoalUpdate(float deltaTime)
        {
            base.GoalUpdate(deltaTime);

            if (IsComplete())
            {
                dialogBox.SetActive(false);
            }
        }
    }
}