/*
 * Description: Holds relevant data about the objectives.
 *              completion status, description, etc.
 */

namespace Service.Framework
{
    public class ObjectiveData
    {
        public string ObjectiveText;
        public bool IsComplete;

        public ObjectiveData(string textEntry)
        {
            ObjectiveText = textEntry;
            IsComplete = false;
        }
    }
}