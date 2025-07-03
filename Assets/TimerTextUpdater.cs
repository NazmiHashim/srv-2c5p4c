using UnityEngine;
using TMPro;

public class TimerTextUpdater : MonoBehaviour
{
    public TextMeshProUGUI timerText; // Drag your "Time" object here
    private float timer = 0f;
    private bool timerRunning = true;

    void Update()
    {
        if (timerRunning)
        {
            timer += Time.deltaTime;

            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);

            if (timerText != null)
            {
                timerText.text = $"Timer: {minutes:00}:{seconds:00}";
            }
        }
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public float GetElapsedTime()
    {
        return timer;
    }
}
