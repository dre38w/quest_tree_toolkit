/*
 * Description: Handles logic specific to quest items
 */

using Service.Framework.Quests;
using UnityEngine;
using Gameplay.System.GameManagement;

namespace Gameplay.System
{
    public class QuestItem : MonoBehaviour
    {
        [SerializeField]
        private QuestItemID itemID;

        [SerializeField]
        private InteractableObject interactable;

        private void Start()
        {
            interactable.OnInteracted.AddListener(PickUpItem);

        }

        /// <summary>
        /// Add the item to the inventory
        /// </summary>
        public void PickUpItem()
        {
            InventoryManager.Instance.AddQuestItem(itemID);
        }

        private void OnDestroy()
        {
            interactable.OnInteracted.RemoveListener(PickUpItem);

        }
    }
}