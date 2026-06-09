using Service.Framework.Goals;
using UnityEngine;

namespace Service.Framework
{
    public class QuestData
    {
        public bool IsComplete;
        public bool IsFailed;
        public QuestID ID;

        public QuestData(QuestID id)
        {
            ID = id;
            IsComplete = false;
            IsFailed = false;
        }
    }
}