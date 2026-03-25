using UnityEngine;
using System;

public enum TimeOfDay { Morning, Afternoon, Evening }

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    [SerializeField] private int currentDay = 1;
    [SerializeField] private TimeOfDay currentTime = TimeOfDay.Morning;

    public int CurrentDay => currentDay;
    public TimeOfDay CurrentTime => currentTime;

    /// <summary>UIManager subscribe event này để cập nhật HUD</summary>
    public event Action onTimeChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    /// <summary>
    /// Gọi khi player hoàn thành hành động của một buổi.
    /// Sáng → Chiều → Tối → Ngày mới (Sáng).
    /// </summary>
    public void AdvanceTime()
    {
        switch (currentTime)
        {
            case TimeOfDay.Morning:
                currentTime = TimeOfDay.Afternoon;
                break;
            case TimeOfDay.Afternoon:
                currentTime = TimeOfDay.Evening;
                break;
            case TimeOfDay.Evening:
                currentDay++;
                currentTime = TimeOfDay.Morning;
                break;
        }
        onTimeChanged?.Invoke();
    }

    /// <summary>Gọi từ SaveSystem khi load game</summary>
    public void SetDayTime(int day, TimeOfDay time)
    {
        currentDay = day;
        currentTime = time;
        onTimeChanged?.Invoke();
    }
}