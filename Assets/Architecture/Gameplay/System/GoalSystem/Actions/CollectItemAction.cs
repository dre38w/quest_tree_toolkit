using Gameplay.System.GameManagement;
using Service.Framework.Goals;
using Service.Framework.Quests;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.System.Actions
{
    public class CollectItemAction : ObjectiveAction
    {
        public UnityEvent OnQuestItemNotFound = new UnityEvent();
        public UnityEvent OnQuestItemFound = new UnityEvent();
        
        [SerializeField]
        private QuestItemID questID;

        public override void InitializeAction()
        {
            for (int i = 0; i < InventoryManager.Instance.QuestItems.Count; i++)
            {
                if (InventoryManager.Instance.QuestItems[i].ItemID != questID)
                {
                    continue;
                }
                SetComplete();
                OnQuestItemFound.Invoke();
                return;
            }
            OnQuestItemNotFound.Invoke();
        }
    }
}