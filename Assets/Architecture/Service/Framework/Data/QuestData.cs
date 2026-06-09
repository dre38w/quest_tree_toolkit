/*
 * Description: Holds data for the quest
 */
using Service.Framework.Goals;

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