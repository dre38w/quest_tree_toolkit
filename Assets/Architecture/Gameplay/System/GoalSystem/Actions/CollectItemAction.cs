/*
 * Description: Handles checking whether or not a quest item is in the player's inventory 
 */

using Gameplay.System.GameManagement;
using Service.Core;
using Service.Framework.Goals;
using Service.Framework.Quests;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.System.Actions
{
    [Submenu("Player/Check For Quest Item")]
    public class CollectItemAction : ObjectiveAction
    {
        [HideInInspector]
        public UnityEvent OnQuestItemNotFound = new UnityEvent();
        [HideInInspector]
        public UnityEvent OnQuestItemFound = new UnityEvent();
        
        //the item to check for
        [SerializeField]
        private QuestItemID questID;

        public override void InitializeAction()
        {
            //iterate over the quest items
            for (int i = 0; i < InventoryManager.Instance.QuestItems.Count; i++)
            {
                //if this isn't it, check again
                if (InventoryManager.Instance.QuestItems[i] != questID)
                {
                    continue;
                }
                //found the item, so complete the action
                SetComplete();
                OnQuestItemFound.Invoke();
                return;
            }
            //didn't find the item, let systems know
            OnQuestItemNotFound.Invoke();
        }
    }
}