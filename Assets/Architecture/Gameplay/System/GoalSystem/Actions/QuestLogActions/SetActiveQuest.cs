using Gameplay.UI;
using Service.Core;
using Service.Framework.Goals;
using UnityEngine;

namespace Gameplay.System.Actions
{
    [Submenu("Quest Log Management/Set Active Quest")]
    public class SetActiveQuest : ObjectiveAction
    {
        [SerializeField]
        private QuestID questID;

        public override void InitializeAction()
        {
            ReferenceRegistry.Instance.MainUI.GetComponent<QuestLogUI>().SetTrackedQuest(questID);
            SetComplete();
        }
    }
}