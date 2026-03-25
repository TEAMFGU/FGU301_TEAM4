using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Day HUD – Session Image")]
    [SerializeField] private Image sessionImage;
    [SerializeField] private Sprite morningSprite;
    [SerializeField] private Sprite afternoonSprite;
    [SerializeField] private Sprite eveningSprite;

    [Header("Day HUD – Day Count")]
    [SerializeField] private TextMeshProUGUI dayCountText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (DayManager.Instance != null)
            DayManager.Instance.onTimeChanged += UpdateDayUI;

        UpdateDayUI();
    }

    private void OnDestroy()
    {
        if (DayManager.Instance != null)
            DayManager.Instance.onTimeChanged -= UpdateDayUI;
    }

    public void UpdateDayUI()
    {
        if (DayManager.Instance == null) return;

        // Cập nhật text ngày
        if (dayCountText != null)
            dayCountText.text = $"{DayManager.Instance.CurrentDay}";

        // Đổi sprite theo buổi
        if (sessionImage != null)
        {
            sessionImage.sprite = DayManager.Instance.CurrentTime switch
            {
                TimeOfDay.Morning => morningSprite,
                TimeOfDay.Afternoon => afternoonSprite,
                TimeOfDay.Evening => eveningSprite,
                _ => morningSprite
            };
        }
    }
}