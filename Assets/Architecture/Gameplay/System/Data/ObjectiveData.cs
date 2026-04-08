using UnityEngine;

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
