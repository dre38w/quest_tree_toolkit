using UnityEngine;

namespace Service.Framework.Goals
{
    [CreateAssetMenu(menuName = "Quest Tree/Objective ID")]
    public class ObjectiveID : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField, HideInInspector]
        private string ownerID;

        public string OwnerID => ownerID;

        public void SetOwner(string id)
        {
            ownerID = id;
        }
#endif
    }
}