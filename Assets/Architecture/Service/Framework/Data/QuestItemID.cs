/*
 * Description: Used to differentiate quest items
 */
using UnityEngine;

namespace Service.Framework.Quests
{
    [CreateAssetMenu(menuName = "Quest Tree/Quest Item ID")]
    public class QuestItemID : ScriptableObject
    {
        public string description;
    }
}