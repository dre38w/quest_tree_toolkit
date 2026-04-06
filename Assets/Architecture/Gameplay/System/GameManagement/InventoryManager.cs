/*
 * Description: Basic inventory script 
 */

using UnityEngine;
using System.Collections.Generic;
using Service.Framework.Quests;

namespace Gameplay.System.GameManagement
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance;

        //The list of quest items as a separate list to make it more efficient to iterate over
        private List<QuestItemID> questItems = new List<QuestItemID>();
        public List<QuestItemID> QuestItems
        {
            get { return questItems; }
            set { questItems = value; }
        }

        //generic list of inventory items that can be used in future
        private List<GameObject> inventoryItems = new List<GameObject>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Used by external systems to add the quest items to the inventory
        /// </summary>
        /// <param name="item"></param>
        public void AddQuestItem(QuestItemID item)
        {
            questItems.Add(item);
        }
    }
}