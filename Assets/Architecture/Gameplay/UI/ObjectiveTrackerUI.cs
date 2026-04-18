using Service.Framework;
using Service.Framework.Goals;
using TMPro;
using UnityEngine;

namespace Gameplay.UI
{
    public class ObjectiveTrackerUI : MonoBehaviour
    {
        private string objectiveID;
        public string ObjectiveID
        {
            get { return objectiveID; }
            set { objectiveID = value; }
        }

        private ObjectiveData objectiveData;

        private TMP_Text objectiveText;

        public void Initialize(ObjectiveData data)
        {
            objectiveText = GetComponent<TMP_Text>();
            objectiveID = data.ID;
            objectiveData = data;
        }

        public void RefreshObjectives(QuestID id)
        {
            if (!objectiveData.IsComplete)
            {
                objectiveText.text = objectiveData.ObjectiveText;
            }
            else
            {
                objectiveText.text = $"<s>{objectiveData.ObjectiveText}</s>";
            }
        }
    }
}