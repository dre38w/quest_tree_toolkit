using Service.Framework.Quests;
using UnityEngine;

public class QuestItem : MonoBehaviour
{
    [SerializeField]
    private QuestItemID itemID;

    public QuestItemID ItemID
    {
        get { return itemID; }
        set { itemID = value; }
    }
    
    /// <summary>
    /// using UI button for now
    /// </summary>
    public void PickUpItem()
    {
        InventoryManager.Instance.AddQuestItem(this);
    }
}
