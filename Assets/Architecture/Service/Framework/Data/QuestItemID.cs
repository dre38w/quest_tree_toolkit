using UnityEngine;

namespace Service.Framework.Quests
{
    [CreateAssetMenu(menuName = "Quest Tree/Quest Item ID")]
    public class QuestItemID : ScriptableObject
    {
        public int id;
        public string description;
    }
}