/*
 * Description: Holds relevant data about the objectives.
 *              completion status, description, etc.
 */

using System;

namespace Service.Framework
{
    public class ObjectiveData
    {
        public string ID;
        public string ObjectiveText;
        public bool IsComplete;

        public ObjectiveData(string textEntry)
        {
            ID = Guid.NewGuid().ToString();
            ObjectiveText = textEntry;
            IsComplete = false;
        }
    }
}