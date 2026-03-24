using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    private NameInputHandler nameInputHandler;

    private void Start()
    {
        // Tìm NameInputHandler trong scene
        nameInputHandler = FindFirstObjectByType<NameInputHandler>();
    }

    // ========== Start Menu Buttons ==========

    public void StartNewGame()
    {
        // Mở panel nhập tên thay vì load scene ngay
        if (nameInputHandler != null)
        {
            nameInputHandler.OpenNameInputPanel();
        }
        else
        {
            Debug.LogError("❌ NameInputHandler không tìm thấy trong scene!");
        }
    }

    public void ContinueGame()
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogWarning("⚠️ SaveSystem chưa được khởi tạo!");
            return;
        }

        if (!SaveSystem.Instance.HasSaveFile())
        {
            Debug.LogWarning("⚠️ Không tìm thấy file save! Hãy bắt đầu game mới.");
            return;
        }

        bool success = SaveSystem.Instance.LoadGame();
        if (success)
        {
            SaveData data = SaveSystem.Instance.GetCurrentSaveData();
            string sceneToLoad = string.IsNullOrEmpty(data.currentScene)
                ? "Scene_Map001_Home"
                : data.currentScene;
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Thoát game!");
        Application.Quit();
    }
}