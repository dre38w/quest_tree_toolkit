
/*
Description: When there is a branching subaction this handles completing all the other non-branching actions/subactions
*/

using System.Linq;

namespace Service.Framework.Goals
{
    public class BranchingSubaction : ObjectiveSubaction
    {
        public override bool HandleBranch(ObjectiveAction sourceAction)
        {
            if (!ObjectiveSubactions.Contains(sourceAction))
            {
                return false;
            }
            //override the current subaction to be the action that was passed
            //which was the branching action and thus needs to activate
            CurrentSubaction = sourceAction;

            //if the the action we are trying to complete is a sub action, then complete all the actions it is managing
            foreach (ObjectiveSubaction sub in ObjectiveSubactions.Cast<ObjectiveSubaction>())
            {
                if (sub != sourceAction)
                {
                    sub.ForceCompleteSubactions();
                }
            }
            //now complete all the other actions besides the one branching
            foreach (ObjectiveAction sub in ObjectiveSubactions)
            {
                if (sub != sourceAction)
                {
                    sub.SetComplete();
                }
            }
            return true;
        }
    }
}