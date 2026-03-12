using UnityEngine;
using UnityEngine.Events;

public class NPC_Interactor : MonoBehaviour
{
    [Header("Data NPC")]
    public NPCData data;                     // Kéo NPC_*.asset vào đây

    [System.Serializable]
    public class DialogueSlot
    {
        [TextArea(2, 5)] public string npcText;
        public string[] playerChoices;       // Lựa chọn của người chơi (nếu có)
        public float affectionChange = 0f;   // + thiện cảm
        public float stressChange = 0f;
        public float studyChange = 0f;       // học lực
    }

    [Header("Hệ thống hội thoại linh hoạt")]
    public DialogueSlot[] dialogueLevels = new DialogueSlot[4]; // Level 1 đến 4

    [Header("Events Cutscene")]
    public UnityEvent onAffection50;     // Cutscene đặc biệt 50%
    public UnityEvent onAffection100;    // Cutscene ending 100%

    private int currentLevel = 1;

    public void Interact()
    {
        if (data == null) return;

        // Tăng thiện cảm +25% mỗi lần tương tác thành công
        data.affection += 25f;
        if (data.affection > 100f) data.affection = 100f;

        // Hiển thị thoại theo level hiện tại
        int index = currentLevel - 1;
        if (index < dialogueLevels.Length && dialogueLevels[index] != null)
        {
            // Sau này bạn sẽ gọi Dialogue System (Yarn Spinner hoặc InteractionManager)
            Debug.Log($"[NPC] {data.npcName}: {dialogueLevels[index].npcText}");
            // Áp dụng hiệu ứng chỉ số
            // GameManager.Instance.AddAffection(data.affectionChange); // bạn sẽ viết sau
        }

        // Kiểm tra mốc cutscene
        if (data.affection >= 50f && !data.unlockedCutscene50)
        {
            data.unlockedCutscene50 = true;
            onAffection50?.Invoke();           // Trigger cutscene đặc biệt
            Debug.Log($"[CUTSCENE] 50% với {data.npcName}");
        }

        if (data.affection >= 100f && !data.unlockedCutscene100)
        {
            data.unlockedCutscene100 = true;
            onAffection100?.Invoke();          // Trigger ending NPC
            Debug.Log($"[ENDING] 100% với {data.npcName}");
        }

        // Tăng level (tối đa 4)
        if (currentLevel < 4) currentLevel++;
    }
}