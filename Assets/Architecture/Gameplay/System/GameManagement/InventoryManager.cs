using UnityEngine;
using System.Collections.Generic;

namespace Gameplay.System.GameManagement
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance;

        private List<QuestItem> questItems = new List<QuestItem>();

        public List<QuestItem> QuestItems
        {
            get { return questItems; }
            set { questItems = value; }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        public void AddQuestItem(QuestItem item)
        {
            questItems.Add(item);
        }
    }
}