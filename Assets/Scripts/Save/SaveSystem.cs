using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    private const string SaveFolder = "Save";
    private const string SaveFile = "savegame.json";

    private SaveData currentSaveData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Lưu game hiện tại vào file JSON
    /// </summary>
    public void SaveGame()
    {
        try
        {
            SaveData data = new SaveData();

            // Lấy toàn bộ dữ liệu từ PlayerDataManager (stats + NPC affinity)
            if (PlayerDataManager.Instance != null)
            {
                PlayerDataManager.Instance.SaveToData(data);
            }

            // Scene hiện tại
            data.currentScene = SceneManager.GetActiveScene().name;

            // Tạo folder Save nếu chưa tồn tại
            string folderPath = Path.Combine(Application.dataPath, SaveFolder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Chuyển đổi sang JSON và lưu
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            string filePath = Path.Combine(folderPath, SaveFile);
            File.WriteAllText(filePath, json);

            currentSaveData = data;
            Debug.Log($"✅ Lưu game thành công tại: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Lỗi khi lưu game: {e.Message}");
        }
    }

    /// <summary>
    /// Đọc game từ file JSON
    /// </summary>
    public bool LoadGame()
    {
        try
        {
            string filePath = Path.Combine(Application.dataPath, SaveFolder, SaveFile);

            if (!File.Exists(filePath))
            {
                Debug.LogWarning("⚠️ Không tìm thấy file save!");
                return false;
            }

            string json = File.ReadAllText(filePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // Ghi lại vào PlayerDataManager
            if (PlayerDataManager.Instance != null)
            {
                PlayerDataManager.Instance.LoadFromSaveData(data);
            }

            currentSaveData = data;
            Debug.Log($"✅ Tải game thành công!");

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Lỗi khi tải game: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// Kiểm tra xem có file save tồn tại hay không
    /// </summary>
    public bool HasSaveFile()
    {
        string filePath = Path.Combine(Application.dataPath, SaveFolder, SaveFile);
        return File.Exists(filePath);
    }

    /// <summary>
    /// Xóa file save (Reset game)
    /// </summary>
    public void DeleteSaveFile()
    {
        try
        {
            string filePath = Path.Combine(Application.dataPath, SaveFolder, SaveFile);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log("✅ Đã xóa file save!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Lỗi khi xóa file save: {e.Message}");
        }
    }

    /// <summary>
    /// Lấy save data hiện tại
    /// </summary>
    public SaveData GetCurrentSaveData()
    {
        return currentSaveData;
    }

    }