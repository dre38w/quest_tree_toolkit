/*
 * Description:  Used to differentiate the quests 
 *              This will help organize multiple goals that may be part of a single quest
 */
using UnityEngine;

namespace Service.Framework.Goals
{
    [CreateAssetMenu(menuName = "Quest Tree/Quest ID")]
    public class QuestID : ScriptableObject
    {
        public string questName;
        public string description;
    }
}