using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject dayManagerPrefab;
    [SerializeField] private GameObject uiManagerPrefab;
    [SerializeField] private GameObject dialogueManagerPrefab;
    [SerializeField] private GameObject menuManagerPrefab;      // ← THÊM MỚI

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
        // Đảm bảo persistent managers (bao gồm SaveSystem + PlayerDataManager) đã được khởi tạo
        InitializePersistentManagers();

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
        if (dayManagerPrefab == null)      Debug.LogError("❌ dayManagerPrefab chưa được gán!");
        if (uiManagerPrefab == null)       Debug.LogError("❌ uiManagerPrefab chưa được gán!");
        if (dialogueManagerPrefab == null) Debug.LogError("❌ dialogueManagerPrefab chưa được gán!");
        if (menuManagerPrefab == null)     Debug.LogError("❌ menuManagerPrefab chưa được gán!");

        // PlayerDataManager & SaveSystem – pure data, không cần prefab
        if (PlayerDataManager.Instance == null)
        {
            GameObject go = new GameObject("PlayerDataManager");
            go.AddComponent<PlayerDataManager>();
        }

        if (SaveSystem.Instance == null)
        {
            GameObject go = new GameObject("SaveSystem");
            go.AddComponent<SaveSystem>();
        }

        if (DayManager.Instance == null && dayManagerPrefab != null)
        {
            GameObject go = Instantiate(dayManagerPrefab);
            go.SetActive(true);
            UnpackPrefabInEditor(go);
        }

        if (UIManager.Instance == null && uiManagerPrefab != null)
        {
            GameObject go = Instantiate(uiManagerPrefab);
            go.SetActive(true);
            UnpackPrefabInEditor(go);
        }

        if (DialogueManager.Instance == null && dialogueManagerPrefab != null)
        {
            GameObject go = Instantiate(dialogueManagerPrefab);
            go.SetActive(true);
            UnpackPrefabInEditor(go);

            if (DialogueManager.Instance == null)
                Debug.LogError("DialogueManager.Instance VẪN null sau Instantiate!");
        }

        // ← THÊM MỚI: khởi tạo MenuManager
        if (MenuManager.Instance == null && menuManagerPrefab != null)
        {
            GameObject go = Instantiate(menuManagerPrefab);
            go.SetActive(true);
            UnpackPrefabInEditor(go);
        }
    }

    private void UnpackPrefabInEditor(GameObject go)
    {
#if UNITY_EDITOR
        if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(go))
        {
            UnityEditor.PrefabUtility.UnpackPrefabInstance(
                go,
                UnityEditor.PrefabUnpackMode.Completely,
                UnityEditor.InteractionMode.AutomatedAction);
        }
#endif
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene("Map00_BenXe");
    }
}