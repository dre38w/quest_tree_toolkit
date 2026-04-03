
/*
Description: Used to differentiate goals
*/

using UnityEngine;

namespace Service.Framework.Goals
{
    [CreateAssetMenu(menuName = "Quest Tree/Goal ID")]
    public class GoalID : ScriptableObject
    {
        public string description;
    }
}