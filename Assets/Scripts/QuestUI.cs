using UnityEngine;
using TMPro;

public class QuestUI : MonoBehaviour
{
    public static QuestUI Instance;
    public GameObject questPanel;
    public Transform questListContainer;
    public GameObject questItemPrefab;

    void Awake() { Instance = this; }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            questPanel.SetActive(!questPanel.activeSelf);
            if (questPanel.activeSelf) UpdateUI();
        }
    }

    public void UpdateUI()
    {
        foreach (Transform child in questListContainer)
            Destroy(child.gameObject);

        foreach (var quest in DailyQuestManager.Instance.todayQuests)
        {
            GameObject item = Instantiate(questItemPrefab, questListContainer);
            bool done = DailyQuestManager.Instance.IsCompleted(quest.questID);

            TMP_Text[] texts = item.GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = (done ? "✅ " : "⬜ ") + quest.questName;
                texts[0].color = done ? Color.green : Color.white;
                texts[1].text = quest.description +
                    " | +" + quest.rewardPoints + " điểm";
            }
        }
    }
}