using Service.Framework.Quests;
using UnityEngine;
using Gameplay.System.GameManagement;
using Service.Framework;

namespace Gameplay.System
{
    public class QuestItem : MonoBehaviour
    {
        [SerializeField]
        private QuestItemID itemID;

        public QuestItemID ItemID
        {
            get { return itemID; }
            set { itemID = value; }
        }

        [SerializeField]
        private InteractableObject interactable;

        private void Start()
        {
            interactable.OnInteracted.AddListener(PickUpItem);

        }

        /// <summary>
        /// using UI button for now
        /// </summary>
        public void PickUpItem()
        {
            InventoryManager.Instance.AddQuestItem(this);
        }

        private void OnDestroy()
        {
            interactable.OnInteracted.RemoveListener(PickUpItem);

        }
    }
}