using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class NPC_INTERACTOR : MonoBehaviour
{
    [Header("Data NPC")]
    public NPCData data;

    [System.Serializable]
    public class ChoiceEffect
    {
        [Tooltip("Thay đổi thiện cảm với NPC này (dương = tăng, âm = giảm)")]
        public float affectionChange = 0f;

        [Tooltip("Thay đổi Stress của player (dương = tăng stress, âm = giảm stress)")]
        public float stressChange = 0f;

        [Tooltip("Thay đổi điểm Học Lực (dương = tăng, âm = giảm)")]
        public float studyChange = 0f;
    }

    [System.Serializable]
    public class DialogueSlot
    {
        [TextArea(2, 5)]
        public string npcText;

        [Tooltip("Danh sách lựa chọn hiển thị cho player (để trống nếu không có choices)")]
        public string[] playerChoices;

        [Tooltip("Hiệu ứng chỉ số theo từng lựa chọn. Index[i] khớp với playerChoices[i].\n" +
                 "Nếu để trống → dùng fallback bên dưới.")]
        public ChoiceEffect[] choiceEffects;

        [Header("Fallback (áp dụng khi không có choices hoặc choiceEffects rỗng)")]
        public float affectionChange = 0f;
        public float stressChange = 0f;
        public float studyChange = 0f;
    }

    [Header("Hệ thống hội thoại linh hoạt")]
    public DialogueSlot[] dialogueLevels = new DialogueSlot[4];

    [Header("Events Cutscene")]
    public UnityEvent onAffection25;
    public UnityEvent onAffection50;
    public UnityEvent onAffection75;
    public UnityEvent onAffection100;

    private int currentLevel = 1;
    private bool isInRange = false;

    private void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.Z))
        {
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsOpen())
                return;

            Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // MAIN INTERACTION
    // ═══════════════════════════════════════════════════════════════════════════

    public void Interact()
    {
        if (data == null)
        {
            Debug.LogWarning("NPCData chưa được gán!");
            return;
        }

        int index = currentLevel - 1;
        DialogueSlot dialogueSlot = null;

        if (index < dialogueLevels.Length && dialogueLevels[index] != null)
            dialogueSlot = dialogueLevels[index];

        if (dialogueSlot == null)
        {
            Debug.LogWarning($"Chưa setup dialogue cho level {currentLevel} của {data.npcName}");
            return;
        }

        string[] dialogueLines = ParseDialogueText(dialogueSlot.npcText);

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDialogue(
                data,
                dialogueLines,
                () => OnDialogueFinished(dialogueSlot),
                dialogueSlot.playerChoices != null && dialogueSlot.playerChoices.Length > 0
            );
        }
        else
        {
            Debug.LogError(
                "❌ DialogueManager.Instance không tồn tại!\n" +
                "→ Nếu đang test trực tiếp từ scene này: kéo Dialogue_Canvas.prefab vào Hierarchy của scene hiện tại.\n" +
                "→ Nếu play từ StartMenu: đảm bảo MenuHandler đã gán Dialogue Manager Prefab trong Inspector."
            );
        }
    }

    private string[] ParseDialogueText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new[] { "[.............................]" };

        string[] raw = text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        var result = new List<string>(raw.Length);
        foreach (string rawLine in raw)
        {
            string trimmed = rawLine.Trim();
            if (!string.IsNullOrEmpty(trimmed))
                result.Add(trimmed);
        }

        if (result.Count == 0)
        {
            Debug.LogWarning($"⚠️ ParseDialogueText: npcText không có dòng hợp lệ. text='{text}'");
            return new[] { "[....................]" };
        }

        return result.ToArray();
    }

    private void OnDialogueFinished(DialogueSlot slot)
    {
        if (data == null) return;

        bool hasChoices = slot.playerChoices != null && slot.playerChoices.Length > 0;

        if (hasChoices && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowChoices(
                slot.playerChoices,
                choiceIndex => ApplyStatsAndProgress(slot, choiceIndex)
            );
        }
        else
        {
            ApplyStatsAndProgress(slot, -1);
        }
    }

    /// <summary>
    /// Apply stats theo lựa chọn.
    /// Ưu tiên choiceEffects[choiceIndex] nếu có, fallback về flat stats nếu không.
    /// </summary>
    private void ApplyStatsAndProgress(DialogueSlot slot, int choiceIndex)
    {
        bool hasEffect = choiceIndex >= 0
                         && slot.choiceEffects != null
                         && choiceIndex < slot.choiceEffects.Length
                         && slot.choiceEffects[choiceIndex] != null;

        float affection = hasEffect ? slot.choiceEffects[choiceIndex].affectionChange : slot.affectionChange;
        float stress = hasEffect ? slot.choiceEffects[choiceIndex].stressChange : slot.stressChange;
        float study = hasEffect ? slot.choiceEffects[choiceIndex].studyChange : slot.studyChange;

        // ─── Thiện cảm NPC ───
        float affectionDelta = affection != 0f ? affection : 25f;
        data.affection = Mathf.Clamp(data.affection + affectionDelta, 0f, 100f);

        // ─── PlayerDataManager stats ───
        if (PlayerDataManager.Instance != null)
        {
            if (affection != 0f)
                PlayerDataManager.Instance.AddNPCAffinity(data.npcName, (int)affection);

            if (stress != 0f)
                PlayerDataManager.Instance.SetStress(
                    PlayerDataManager.Instance.GetStress() + (int)stress);

            if (study != 0f)
                PlayerDataManager.Instance.SetAcademicScore(
                    PlayerDataManager.Instance.GetAcademicScore() + (int)study);
        }

        // ─── Kiểm tra mốc affection ───
        CheckAffectionMilestones();

        // ─── Tăng level (max 4) ───
        if (currentLevel < 4)
            currentLevel++;
    }

    private void CheckAffectionMilestones()
    {
        if (data.affection >= 25f && !data.unlockedCutscene25)
        {
            data.unlockedCutscene25 = true;
            onAffection25?.Invoke();
        }
        if (data.affection >= 50f && !data.unlockedCutscene50)
        {
            data.unlockedCutscene50 = true;
            onAffection50?.Invoke();
        }
        if (data.affection >= 75f && !data.unlockedCutscene75)
        {
            data.unlockedCutscene75 = true;
            onAffection75?.Invoke();
        }
        if (data.affection >= 100f && !data.unlockedCutscene100)
        {
            data.unlockedCutscene100 = true;
            onAffection100?.Invoke();
        }
    }
}