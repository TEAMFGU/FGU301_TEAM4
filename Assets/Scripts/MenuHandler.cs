using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject dayManagerPrefab;
    [SerializeField] private GameObject uiManagerPrefab;
    [SerializeField] private GameObject dialogueManagerPrefab;
    private NameInputHandler nameInputHandler;

    private void Start()
    {
        nameInputHandler = FindFirstObjectByType<NameInputHandler>();
    }

    public void StartNewGame()
    {
        if (nameInputHandler != null)
            nameInputHandler.OpenNameInputPanel();
        else
            Debug.LogError("NameInputHandler không tìm thấy trong scene!");
    }

    public void ContinueGame()
    {
        if (SaveSystem.Instance == null)
        {
            Debug.LogWarning("SaveSystem chưa được khởi tạo!");
            return;
        }

        if (!SaveSystem.Instance.HasSaveFile())
        {
            Debug.LogWarning("Không tìm thấy file save! Hãy bắt đầu game mới.");
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
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void InitializePersistentManagers()
    {
        // Null-check prefabs để phát hiện lỗi chưa gán Inspector
        if (dayManagerPrefab == null)   Debug.LogError("❌ dayManagerPrefab chưa được gán trong Inspector!");
        if (uiManagerPrefab == null)    Debug.LogError("❌ uiManagerPrefab chưa được gán trong Inspector!");
        if (dialogueManagerPrefab == null) Debug.LogError("❌ dialogueManagerPrefab chưa được gán trong Inspector!");

        if (DayManager.Instance == null && dayManagerPrefab != null)
        {
            GameObject go = Instantiate(dayManagerPrefab);
            go.SetActive(true); // Đảm bảo Awake() chạy dù prefab root inactive
            UnpackPrefabInEditor(go);
            Debug.Log("DayManager instantiated");
        }

        if (UIManager.Instance == null && uiManagerPrefab != null)
        {
            GameObject go = Instantiate(uiManagerPrefab);
            go.SetActive(true);
            UnpackPrefabInEditor(go);
            Debug.Log("UIManager instantiated");
        }

        if (DialogueManager.Instance == null && dialogueManagerPrefab != null)
        {
            GameObject go = Instantiate(dialogueManagerPrefab);
            go.SetActive(true); // QUAN TRỌNG: prefab root phải active để Awake() gán Instance
            UnpackPrefabInEditor(go);
            Debug.Log("DialogueManager instantiated");

            // Kiểm tra lại sau khi instantiate
            if (DialogueManager.Instance == null)
            {
                Debug.LogError("DialogueManager.Instance VẪN null sau Instantiate! Kiểm tra script DialogueManager có gắn trên root GameObject của prefab không.");
            }
        }
    }

    /// <summary>
    /// Unpack prefab instance trong Editor để mở khóa chỉnh sửa child objects trong Play mode.
    /// Không ảnh hưởng khi build game.
    /// </summary>
    private void UnpackPrefabInEditor(GameObject go)
    {
#if UNITY_EDITOR
        if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(go))
        {
            UnityEditor.PrefabUtility.UnpackPrefabInstance(go, UnityEditor.PrefabUnpackMode.Completely, UnityEditor.InteractionMode.AutomatedAction);
            Debug.Log($"Đã unpack prefab: {go.name} – child objects có thể chỉnh sửa tự do.");
        }
#endif
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene("Map00_BenXe");
    }
}