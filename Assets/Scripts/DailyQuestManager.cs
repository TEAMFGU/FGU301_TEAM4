using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DailyQuestManager : MonoBehaviour
{
    public static DailyQuestManager Instance;

    public QuestData[] allQuests;
    public int questPerDay = 3;

    public List<QuestData> todayQuests = new List<QuestData>();
    public List<string> completedIDs = new List<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); return; }

        // Lần đầu chạy game → tạo quest
        if (PlayerPrefs.GetString("TodayQuests", "") == "")
            GenerateDailyQuests();
        else
        {
            LoadTodayQuests();
            LoadCompletedQuests();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Gọi hàm này khi player ngủ
    public void OnSleep()
    {
        PlayerPrefs.DeleteKey("TodayQuests");
        PlayerPrefs.DeleteKey("CompletedQuests");
        completedIDs.Clear();
        GenerateDailyQuests();
        if (QuestUI.Instance != null) QuestUI.Instance.UpdateUI();
        Debug.Log("🌙 Ngày mới! Quest đã reset!");
    }

    void GenerateDailyQuests()
    {
        todayQuests.Clear();
        List<QuestData> pool = new List<QuestData>(allQuests);

        // Xáo trộn ngẫu nhiên
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var tmp = pool[i]; pool[i] = pool[j]; pool[j] = tmp;
        }

        for (int i = 0; i < Mathf.Min(questPerDay, pool.Count); i++)
            todayQuests.Add(pool[i]);

        // Lưu
        string ids = "";
        foreach (var q in todayQuests) ids += q.questID + ",";
        PlayerPrefs.SetString("TodayQuests", ids);
    }

    void LoadTodayQuests()
    {
        todayQuests.Clear();
        string ids = PlayerPrefs.GetString("TodayQuests", "");
        foreach (string id in ids.Split(','))
            foreach (var q in allQuests)
                if (q.questID == id) todayQuests.Add(q);
    }

    void LoadCompletedQuests()
    {
        completedIDs.Clear();
        string saved = PlayerPrefs.GetString("CompletedQuests", "");
        foreach (string id in saved.Split(','))
            if (id != "") completedIDs.Add(id);
    }

    // Gọi khi đến map mới
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var quest in todayQuests)
        {
            if (quest.questType == QuestData.QuestType.VisitScene &&
                quest.targetScene == scene.name &&
                !completedIDs.Contains(quest.questID))
            {
                CompleteQuest(quest);
            }
        }
    }

    // Gọi từ NPC khi player nói chuyện
    public void OnTalkToNPC(string npcName)
    {
        foreach (var quest in todayQuests)
        {
            if (quest.questType == QuestData.QuestType.TalkToNPC &&
                quest.targetNPCName == npcName &&
                !completedIDs.Contains(quest.questID))
            {
                CompleteQuest(quest);
            }
        }
    }

    void CompleteQuest(QuestData quest)
    {
        completedIDs.Add(quest.questID);
        string saved = PlayerPrefs.GetString("CompletedQuests", "");
        PlayerPrefs.SetString("CompletedQuests", saved + quest.questID + ",");

        if (QuestUI.Instance != null) QuestUI.Instance.UpdateUI();

        Debug.Log("✅ Hoàn thành: " + quest.questName);
    }

    public bool IsCompleted(string id) => completedIDs.Contains(id);
    // Gọi khi tương tác với object
    public void OnInteractObject(string objectName)
    {
        foreach (var quest in todayQuests)
        {
            if (quest.questType == QuestData.QuestType.InteractObject &&
                quest.targetObjectName == objectName &&
                !completedIDs.Contains(quest.questID))
            {
                CompleteQuest(quest);
            }
        }
    }
}