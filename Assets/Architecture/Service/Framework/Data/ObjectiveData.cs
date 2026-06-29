/*
 * Description: Holds relevant data about the objectives.
 *              completion status, description, etc.
 */

using Service.Framework.Goals;
using System;
using System.Collections.Generic;

namespace Service.Framework
{
    public class ObjectiveData
    {
        /// <summary>
        /// The SO ID used for gameplay such as referencing a past action, etc.
        /// </summary>
        public ObjectiveID TrackedID;
        public string ID;
        public string ObjectiveText;
        public bool IsComplete;
        public bool IsFailed;
        public bool IsSubObjective;

        //The ID of this objective's parent objective.  Useful for conditional branches
        public string ParentObjectiveID;
        //if this is a parent objective, reference a list of the IDs of any child objectives
        public List<string> SubObjectivesIDs = new List<string>();

        public ObjectiveData(string textEntry, bool isSubObjective, string parentID = null, ObjectiveID objectiveID = null)
        {
            //set a unique GUID to allow for easy and foolproof ID marking
            ID = Guid.NewGuid().ToString();
            TrackedID = objectiveID;
            ObjectiveText = textEntry;
            IsComplete = false;
            IsSubObjective = isSubObjective;
            ParentObjectiveID = parentID;
        }
    }
}