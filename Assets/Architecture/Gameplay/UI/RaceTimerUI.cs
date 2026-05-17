using UnityEngine;
using UnityEngine.UI;

public class RaceTimerUI : MonoBehaviour
{
    [SerializeField]
    private Text timerText;

    public void DisplayTimer(float displayTime)
    {
        timerText.text = displayTime.ToString("F2");
    }
}
