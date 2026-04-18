/*
 * Description: Holds relevant data about the objectives.
 *              completion status, description, etc.
 */

using System;
using System.Collections.Generic;

namespace Service.Framework
{
    public class ObjectiveData
    {
        public string ID;
        public string ObjectiveText;
        public bool IsComplete;
        public bool IsSubObjective;

        public string ParentObjectiveID;
        public List<string> SubObjectivesIDs = new List<string>();

        public ObjectiveData(string textEntry, bool isSubObjective, string parentID = null)
        {
            ID = Guid.NewGuid().ToString();
            ObjectiveText = textEntry;
            IsComplete = false;
            IsSubObjective = isSubObjective;
            ParentObjectiveID = parentID;
        }
    }
}