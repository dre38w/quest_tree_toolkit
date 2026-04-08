using TMPro;
using UnityEngine;

public class ObjectiveEntryUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text objectiveText;

    public void SetText(ObjectiveData data)
    {
        objectiveText.text = data.IsComplete ? $"<s>{data.ObjectiveText}</s>" : data.ObjectiveText;
    }
}
