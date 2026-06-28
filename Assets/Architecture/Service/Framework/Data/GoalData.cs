using Service.Framework.Goals;

namespace Service.Framework
{
    public class GoalData
    {
        public bool IsComplete;
        public bool IsFailed;
        public GoalID ID;

        public GoalData(GoalID id)
        {
            ID = id;
            IsComplete = false;
            IsFailed = false;
        }
    }
}