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
        public bool IsFailed;
        public bool IsSubObjective;

        //The ID of this objective's parent objective.  Useful for conditional branches
        public string ParentObjectiveID;
        //if this is a parent objective, reference a list of the IDs of any child objectives
        public List<string> SubObjectivesIDs = new List<string>();

        public ObjectiveData(string textEntry, bool isSubObjective, string parentID = null)
        {
            //set a unique GUID to allow for easy and foolproof ID marking
            if (ID == null)
            {
                ID = Guid.NewGuid().ToString();
            }
            ObjectiveText = textEntry;
            IsComplete = false;
            IsSubObjective = isSubObjective;
            ParentObjectiveID = parentID;
        }
    }
}