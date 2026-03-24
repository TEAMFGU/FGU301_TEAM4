using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NameInputHandler : MonoBehaviour
{
    [SerializeField] private GameObject inputPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private void OnEnable()
    {
        // Khi panel hiển thị, focus vào InputField
        if (inputField != null)
        {
            inputField.Select();
            inputField.ActivateInputField();
        }
    }

    private void Start()
    {
        // Gán callback cho buttons
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmButtonPressed);
        
        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancelButtonPressed);

        // Gán callback khi nhấn Enter trong InputField
        if (inputField != null)
            inputField.onSubmit.AddListener(OnInputSubmit);
    }

    /// <summary>
    /// Nhấn nút Xác Nhận hoặc Enter
    /// </summary>
    private void OnConfirmButtonPressed()
    {
        ConfirmPlayerName();
    }

    private void OnInputSubmit(string text)
    {
        ConfirmPlayerName();
    }

    /// <summary>
    /// Xác nhận tên người chơi
    /// </summary>
    private void ConfirmPlayerName()
    {
        string playerName = inputField.text.Trim();

        // Validate: không cho phép tên rỗng
        if (string.IsNullOrWhiteSpace(playerName))
        {
            Debug.LogWarning("❌ Tên người chơi không được để trống!");
            inputField.text = "";
            inputField.Select();
            return;
        }

        // Lưu tên vào PlayerPrefs
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
        Debug.Log($"✅ Lưu tên người chơi: {playerName}");

        // Load scene chính (Map00_BenXe - intro)
        SceneManager.LoadScene("Map00_BenXe");
    }

    /// <summary>
    /// Nhấn nút Hủy - quay lại menu
    /// </summary>
    private void OnCancelButtonPressed()
    {
        inputField.text = "";
        inputPanel.SetActive(false);
        if (menuPanel != null) menuPanel.SetActive(true);
    }

    /// <summary>
    /// Mở panel nhập tên
    /// </summary>
    public void OpenNameInputPanel()
    {
        inputField.text = "";
        if (menuPanel != null) menuPanel.SetActive(false);
        inputPanel.SetActive(true);
        inputField.Select();
        inputField.ActivateInputField();
    }
}